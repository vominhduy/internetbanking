<template>
  <b-card class="bcard-shadow">
    <h1>Tạo tài khoản khách hàng</h1>
    <b-form @submit.stop.prevent="onSubmit" v-if="show">
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Tên người dùng" label-for="Name">
        <b-form-input id="Name" name="Name" v-validate="{required:true}" v-model="user.Name" :state="validateState('Name')"
          aria-describedby="NameFeedback"></b-form-input>
        <b-form-invalid-feedback id="NameFeedback">Tên người dùng không được để trống!</b-form-invalid-feedback>
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Giới tính" label-for="Gender">
        <b-form-select id="Gender" v-model="user.Gender" :options="Genders"></b-form-select>
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Địa chỉ" label-for="Address">
        <b-form-input id="Address" name="Address" v-model="user.Address" v-validate="{required:true}" :state="validateState('Address')"
          aria-describedby="AddressFeedback"></b-form-input>
        <b-form-invalid-feedback id="AddressFeedback">Địa chỉ không được để trống!</b-form-invalid-feedback>
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Email" label-for="Email">
        <b-form-input id="Email" :type="'email'" name="Email" v-validate="'required|email'" v-model="user.Email" :state="validateState('Email')"
          aria-describedby="EmailFeedback"></b-form-input>
        <b-form-invalid-feedback id="EmailFeedback">Email không đúng định dạng!</b-form-invalid-feedback>
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Số điện thoại" label-for="Phone">
        <b-form-input id="Phone" :type="'number'" name="Phone" v-validate="{required:true}" v-model="user.Phone" :state="validateState('Phone')"
          aria-describedby="PhoneFeedback"></b-form-input>
        <b-form-invalid-feedback id="PhoneFeedback">Số điện thoại không được để trống!</b-form-invalid-feedback>
      </b-form-group>
      <b-form-group>
        <b-row>
          <b-col>
            <b-button block type="submit" variant="success">Thêm</b-button>
          </b-col>
          <b-col>
            <b-button block variant="danger" @click.prevent="canceled">Hủy</b-button>
          </b-col>
        </b-row>
      </b-form-group>
    </b-form>
    <b-modal ref="respone" title="Thông tin người dùng">
      <b-row>
        <b-col>
          <label>Số tài khoản</label>
        </b-col>
        <b-col>
          <label class="font-weight-bold pt-0">{{respone.AccountNumber}}</label>
        </b-col>
      </b-row>
      <b-row>
        <b-col>
          <label>Tên</label>
        </b-col>
        <b-col>
          <label class="font-weight-bold pt-0">{{respone.Name}}</label>
        </b-col>
      </b-row>
      <b-row>
        <b-col>
          <label>Tên đăng nhập</label>
        </b-col>
        <b-col>
          <label class="font-weight-bold pt-0">{{respone.Username}}</label>
        </b-col>
      </b-row>
      <b-row>
        <b-col>
          <label>Mật khẩu</label>
        </b-col>
        <b-col>
          <label class="font-weight-bold pt-0">{{respone.Password}}</label>
        </b-col>
      </b-row>
      <b-row>
        <b-col>
          <label>Giới tính</label>
        </b-col>
        <b-col>
          <label
            class="font-weight-bold pt-0"
          >{{respone.Gender == 1 ? 'Nam' : (respone.Gender == 2 ? 'Nữ' : 'Khác')}}</label>
        </b-col>
      </b-row>
      <b-row>
        <b-col>
          <label>Địa chỉ</label>
        </b-col>
        <b-col>
          <label class="font-weight-bold pt-0">{{respone.Address}}</label>
        </b-col>
      </b-row>
      <b-row>
        <b-col>
          <label>Email</label>
        </b-col>
        <b-col>
          <label class="font-weight-bold pt-0">{{respone.Email}}</label>
        </b-col>
      </b-row>
      <b-row>
        <b-col>
          <label>Số điện thoại</label>
        </b-col>
        <b-col>
          <label class="font-weight-bold pt-0">{{respone.Phone}}</label>
        </b-col>
      </b-row>
      <template v-slot:modal-footer>
        <div hidden></div>
      </template>
    </b-modal>
  </b-card>
</template>

<style scoped>
.bcard-shadow {
  margin-top: 15px;
  box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
}
</style>

<script>
import apiHelper from '../../helper/call_api'

export default {
  name: "CreateUser",
  data() {
    return {
      respone: {},
      user: {
        Phone: null,
        Name: null,
        Email: null,
        Address: null,
        Gender: 1
      },
      Genders: [
        { value: 1, text: "Nam" },
        { value: 2, text: "Nữ" },
        { value: 3, text: "Khác" }
      ],
      show: true
    };
  },
  methods: {
    onSubmit(evt) {
      evt.preventDefault();
      this.$validator.validateAll().then(result => {
        if (!result) {
          return;
        }
        apiHelper
          .call_api(`employees`, "post", this.user)
          .then(res => {
            this.respone = res.data;
            this.$refs["respone"].show();
          })
          .catch(err => {
            this.empty = true;
            console.log(err);
          });
    })},
    canceled(evt) {
      evt.preventDefault()
      // Reset our form values
      this.user = {};
      this.user.Gender = 1;
      // Trick to reset/clear native browser form validation state
      this.show = false
      this.$nextTick(() => {
        this.show = true
      })
    },
    validateState(ref) {
      if (
        this.veeFields[ref] &&
        (this.veeFields[ref].dirty || this.veeFields[ref].validated)
      ) {
        return !this.veeErrors.has(ref);
      }
      return null;
    },
  }
};
</script>
