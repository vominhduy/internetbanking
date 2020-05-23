<template>
  <b-card class="bcard-shadow">
    <h1>Lịch sử giao dịch</h1>
    <b-form @submit.stop.prevent="onSubmit">
      <b-form-group>
        <b-row>
          <b-col>
            <b-button block type="submit" variant="success">Tra cứu</b-button>
          </b-col>
        </b-row>
      </b-form-group>
    </b-form>
    <div v-if="statusOk">
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
      <b-tabs content-class="mt-3">
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
import apiHelper from '../../helper/call_api'

export default {
  name: "UserHistories",
  components: {
    HistoryDeptIn,
    HistoryIn,
    HistoryDeptOut,
    HistoryOut
  },
  data() {
    return {
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
      hisDeptOuts: []
    };
  },

  methods: {
    onSubmit(evt) {
      evt.preventDefault();
      this.$validator.validateAll().then(result => {
        if (!result) {
          return;
        }
     // if (this.user.AccountNumber == "" && this.user.Username == "") {
     ///   return;
     // }

      if (this.type == 1) this.user.UserName = this.valueType;
        else this.user.AccountNumber = this.valueType;
      
      console.log(this.user);
      apiHelper
          .call_api(`Users`, "get", '')
          .then(res => {
            if(res.data){
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
                .call_api(`users/histories/in`, "get", '')
                .then(res => {
                  this.hisIns = res.data;
                })
                .catch(err => {
                  console.error(err);
                });
              // get histories out
              apiHelper
                .call_api(`users/histories/out`, "get", '')
                .then(res => {
                  this.hisOuts = res.data;
                })
                .catch(err => {
                  console.error(err);
                });
              // get histories dept in
              apiHelper
                .call_api(`users/histories/depts/in`, "get", '')
                .then(res => {
                  this.hisDeptIns = res.data;
                })
                .catch(err => {
                  console.error(err);
                });
              // get histories dept out  
              apiHelper
                .call_api(`users/histories/depts/out`, "get", '')
                .then(res => {
                  this.hisDeptOuts = res.data;
                })
                .catch(err => {
                  console.error(err);
                });
            }
          })
          .catch(err => {
            this.statusOk = false;
            console.error(err);
            this.responeMessage = "Không tìm thấy thông tin tài khoản!";
            this.$refs["respone"].show();
          });
    })},
  }
};
</script>

<style scoped>
.bcard-shadow {
  margin-top: 15px;
  box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
}
</style>