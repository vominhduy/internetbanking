<template>
  <div class="createuser">
    <h1>
      Tạo tài khoản khách hàng
    </h1>
    <b-form @submit.stop.prevent="onSubmit">
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Tên người dùng" label-for="Name">
        <b-form-input id="Name" v-model="user.Name"></b-form-input>
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Giới tính" label-for="Gender">
        <b-form-select id="Gender" v-model="user.Gender" :options="Genders"></b-form-select>
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Địa chỉ" label-for="Address">
        <b-form-input id="Address" v-model="user.Address"></b-form-input>
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Email" label-for="Email">
        <b-form-input id="Email" :type="'email'" v-model="user.Email"></b-form-input>
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Số điện thoại" label-for="Phone">
        <b-form-input id="Phone" :type="'number'" v-model="user.Phone"></b-form-input>
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
          <label class="font-weight-bold pt-0">{{respone.AccountNumber}}</label>
        </b-col>
      </b-row>
      <template v-slot:modal-footer>
        <div hidden></div>
      </template>
    </b-modal>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "CreateUser",
  data() {
    return {
      respone: {},
      user: {
        Phone: "",
        Name: "",
        Email: "",
        Address: "",
        Gender: 1
      },
      Genders: [
        { value: 1, text: "Nam" },
        { value: 2, text: "Nữ" },
        { value: 3, text: "Khác" }
      ]
    };
  },
  methods: {
    onSubmit(evt) {
      evt.preventDefault();
      axios
        .post(`employees`, this.user)
        .then(res => {
          this.respone = res.data;
          this.$refs["respone"].show();
        })
        .catch(err => {
          this.empty = true;
          console.log(err);
        });
    },
    canceled() {
      this.user = {};
      this.user.Gender = 1;
    }
  }
};
</script>
