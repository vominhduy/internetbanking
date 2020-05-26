<template>
  <b-card class="bcard-shadow">
    <h1>Lịch sử giao dịch</h1>
    <b-form @submit.stop.prevent="onSubmit">
      <div style="display:none;">
        <b-form-group label-cols-sm="12" label-cols-md="4" label="Loại" label-for="type">
          <b-form-select id="type" v-model="type" :options="types"></b-form-select>
        </b-form-group>
        <b-form-group label-cols-sm="12" label-cols-md="4" label="Tên đăng nhập/Số tài khoản">
          <b-form-input
            id="value"
            name="value"
            v-model="valueType"
            v-validate="{required:true}"
            :state="validateState('value')"
            aria-describedby="valuefeedback"
          ></b-form-input>
          <b-form-invalid-feedback
            v-if="type == 1"
            id="valuefeedback"
          >Tên đăng nhập không được để trống!</b-form-invalid-feedback>
          <b-form-invalid-feedback
            v-if="type == 2"
            id="valuefeedback"
          >Số tài khoản không được để trống!</b-form-invalid-feedback>
          <v-select label="vmName" :options="users"></v-select>
          <!-- <div v-show="type == 1">
          <b-form-input name="value" id="value" v-model="user.Username" v-validate="{required: true}" :state="validateState('value')"
          aria-describedby="input-live-feedback"></b-form-input>
          <b-form-invalid-feedback id="input-live-feedback">Tên đăng nhập không được để trống!</b-form-invalid-feedback>
        </div>
        <div v-show="type == 2">
          <b-form-input name="value1" id="value1" v-model="user.AccountNumber" v-validate="{required:true}" :state="validateState('value1')"
          aria-describedby="input-1-live-feedback"></b-form-input>
          <b-form-invalid-feedback id="input-1-live-feedback">Số tài khoản không được để trống!</b-form-invalid-feedback>
          </div>-->
        </b-form-group>
      </div>

      <b-form-group>
        <b-row>
          <b-col sm="9">
            <v-select label="Name" :options="users" v-model="selectedUser">
              <template #no-options="{}">Không tìm thấy!</template>
              <template #option="{ Name, AccountNumber, Email }">
                <h5 style="margin: 0">{{ Name }}</h5>
                <em>{{ AccountNumber }} - {{ Email }}</em>
              </template>
            </v-select>
            <div
              class="invalid-feedback1"
              style="display:block;"
              v-show="selectedUser == null"
            >Tên đăng nhập không được để trống!</div>
          </b-col>
          <b-col sm="3">
            <b-button block type="submit" size="sm" variant="success">Tra cứu</b-button>
          </b-col>
        </b-row>
      </b-form-group>
    </b-form>
    <div v-show="statusOk">
      <h3>Thông tin tài khoản</h3>
      <p>
        Tên:
        <span class="font-weight-bold">{{user.Name}}</span>
      </p>
      <p>
        Tên đăng nhập:
        <span class="font-weight-bold">{{user.Username}}</span>
      </p>
      <p>
        Số tài khoản:
        <span class="font-weight-bold">{{user.AccountNumber}}</span>
      </p>
      <p>
        Giới tính:
        <span
          class="font-weight-bold"
        >{{user.Gender == 1 ? "Nam" : (user.Gender == 2 ? "Nữ": "Khác")}}</span>
      </p>
      <p>
        Địa chỉ:
        <span class="font-weight-bold">{{user.Address}}</span>
      </p>
      <p>
        Số điện thoại:
        <span class="font-weight-bold">{{user.Phone}}</span>
      </p>
      <b-tabs content-class="mt-3" v-show="countCallApi == 4">
        <b-tab title="Nhận tiền" active>
          <HistoryIn :histories1="hisIns" />
        </b-tab>
        <b-tab title="Chuyển khoản">
          <HistoryOut :histories2="hisOuts" />
        </b-tab>
        <b-tab title="Nhắc nợ - nhận">
          <HistoryDeptIn :histories3="hisDeptIns" />
        </b-tab>
        <b-tab title="Nhắc nợ - trả">
          <HistoryDeptOut :histories4="hisDeptOuts" />
        </b-tab>
      </b-tabs>
    </div>
    <div class="text-center" v-show="countCallApi < 4 && showSpinner">
      <b-spinner label="Large Spinner"></b-spinner>
    </div>
    <b-modal ref="respone" title="Thông báo">
      <b-row>
        <b-col>
          <label>{{responeMessage}}</label>
        </b-col>
      </b-row>
      <template v-slot:modal-footer>
        <div hidden></div>
      </template>
    </b-modal>
  </b-card>
</template>

<script>
import HistoryDeptIn from "../../components/Employee/HistoryDeptIn.vue";
import HistoryIn from "../../components/Employee/HistoryIn.vue";
import HistoryDeptOut from "../../components/Employee/HistoryDeptOut.vue";
import HistoryOut from "../../components/Employee/HistoryOut.vue";
import apiHelper from "../../helper/call_api";
import "vue-select/dist/vue-select.css";

export default {
  name: "History",
  components: {
    HistoryDeptIn,
    HistoryIn,
    HistoryDeptOut,
    HistoryOut
  },
  data() {
    return {
      showSpinner: false,
      statusOk: false,
      send: "",
      responeMessage: "",
      payInfo: {
        UserName: "",
        AccountNumber: "",
        Money: 0
      },
      valueType: "",
      type: 1,
      types: [
        { value: 1, text: "Tên đăng nhập" },
        { value: 2, text: "Số tài khoản" }
      ],
      user: {
        AccountNumber: "",
        Username: ""
      },
      hisIns: [],
      hisOuts: [],
      hisDeptIns: [],
      hisDeptOuts: [],
      users: [],
      selectedUser: {},
      countCallApi: 0
    };
  },
  beforeMount: function() {
    this.$nextTick(function() {
      apiHelper
        .call_api(`employees/users`, "get", "")
        .then(res => {
          this.users = res.data;
          this.users.forEach(element => {
            element.VmName =
              element.AccountNumber +
              " - " +
              element.Name +
              " - " +
              element.Phone;
          });
          if (this.users.length > 0) this.selectedUser = this.users[0];
        })
        .catch(err => {
          console.error(err);
        });
    });
  },
  methods: {
    onSubmit(evt) {
      evt.preventDefault();
      if (this.selectedUser == null) return;
      this.countCallApi = 0;
      this.statusOk = false;
      this.showSpinner = true;
      // if (this.user.AccountNumber == "" && this.user.Username == "") {
      ///   return;
      // }
      this.user = {};
      this.user.UserName = this.selectedUser.Username;
      this.user.AccountNumber = this.selectedUser.AccountNumber;

      console.log(this.user);
      apiHelper
        .call_api(`employees/users`, "post", this.user)
        .then(res => {
          this.user.Username = res.data.Username;
          this.user.AccountNumber = res.data.AccountNumber;
          this.user.Id = res.data.Id;
          this.user.Name = res.data.Name;
          this.user.Gender = res.data.Gender;
          this.user.Address = res.data.Address;
          this.user.Phone = res.data.Phone;
          this.user.IsPayIn = res.data.IsPayIn;
          this.user.BankName = res.data.BankName;
          this.statusOk = true;
          // get histories in
          apiHelper
            .call_api(`employees/histories/${this.user.Id}/in`, "get", "")
            .then(res => {
              this.hisIns = res.data;
              this.countCallApi++;
            })
            .catch(err => {
              console.error(err);
              this.countCallApi++;
            });
          // get histories out
          apiHelper
            .call_api(`employees/histories/${this.user.Id}/out`, "get", "")
            .then(res => {
              this.hisOuts = res.data;
              this.countCallApi++;
            })
            .catch(err => {
              console.error(err);
              this.countCallApi++;
            });
          // get histories dept in
          apiHelper
            .call_api(`employees/histories/${this.user.Id}/dept/in`, "get", "")
            .then(res => {
              this.hisDeptIns = res.data;
              this.countCallApi++;
            })
            .catch(err => {
              console.error(err);
              this.countCallApi++;
            });
          // get histories dept out
          apiHelper
            .call_api(`employees/histories/${this.user.Id}/dept/out`, "get", "")
            .then(res => {
              this.hisDeptOuts = res.data;
              this.countCallApi++;
            })
            .catch(err => {
              console.error(err);
              this.countCallApi++;
            });
        })
        .catch(err => {
          this.statusOk = false;
          console.error(err);
          this.responeMessage = "Không tìm thấy thông tin tài khoản!";
          this.$refs["respone"].show();
        });
    },
    validateState(ref) {
      if (
        this.veeFields[ref] &&
        (this.veeFields[ref].dirty || this.veeFields[ref].validated)
      ) {
        return !this.veeErrors.has(ref);
      }
      return null;
    }
  }
};
</script>

<style scoped>
.bcard-shadow {
  margin-top: 15px;
  box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
}
.invalid-feedback1 {
  width: 100%;
  margin-top: 0.25rem;
  font-size: 80%;
  color: #dc3545;
}
</style>