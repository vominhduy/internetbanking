<template>
  <div class="History">
    <h1>Lịch sử giao dịch</h1>
    <b-form @submit.stop.prevent="onSubmit">
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Loại" label-for="type">
        <b-form-select id="type" v-model="type" :options="types"></b-form-select>
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Tên đăng nhập/Số tài khoản">
        <b-form-input v-if="type == 1" id="value" v-model="user.Username"></b-form-input>
        <b-form-input v-if="type == 2" id="value1" v-model="user.AccountNumber"></b-form-input>
      </b-form-group>
      <b-form-group>
        <b-row>
          <b-col>
            <b-button block type="submit" variant="success">Tra cứu</b-button>
          </b-col>
          <b-col>
            <b-button block variant="danger" @click.prevent="canceled">Hủy</b-button>
          </b-col>
        </b-row>
      </b-form-group>
    </b-form>
    <div v-if="statusOk">
      <h2>Thông tin tài khoản</h2>
      <p>Tên: {{user.Name}}</p>
      <p>Tên đăng nhập: {{user.Userame}}</p>
      <p>Số tài khoản: {{user.AccountNumber}}</p>
      <p>Giới tính: {{user.Gender == 1 ? "Name" : (user.Gender == 2 ? "Nữ": "Khác")}}</p>
      <p>Địa chỉ: {{user.Address}}</p>
      <p>Số điện thoại: {{user.Phone}}</p>
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
  </div>
</template>

<script>
import axios from "axios";
import HistoryDeptIn from "../../components/Employee/HistoryDeptIn.vue";
import HistoryIn from "../../components/Employee/HistoryIn.vue";
import HistoryDeptOut from "../../components/Employee/HistoryDeptOut.vue";
import HistoryOut from "../../components/Employee/HistoryOut.vue";

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
      
      if (this.user.AccountNumber == "" && this.user.Username == "")
      {
        console.log("dfdsfdsfdf");
        return;
      }

      if (this.type == 1) {
        this.user.AccountNumber = "";
      } else {
        this.user.Username = "";
      }

      

      console.log("get user", this.user);
      axios
        .post(`employees/users`, this.user)
        .then(res => {
          this.user.Username = res.data.Username;
          this.user.AccountNumber = res.data.AccountNumber;
          this.user.Id = res.data.Id;
          this.user.Name = res.data.Name;
          this.user.Gender = res.data.Gender;
          this.user.Address = res.data.Address;
          this.user.Phone = res.data.Phone;
          this.statusOk = true;
          console.log("user", res.data);
          // get histories in
          axios
            .get("employees/histories/" + this.user.Id + "/in")
            .then(res => {
              this.hisIns = res.data;
              console.log('data', this.hisIns);
            })
            .catch(err => {
              console.log(err);
            });
          // get histories out
          axios
            .get("employees/histories/" + this.user.Id + "/out")
            .then(res => {
              this.hisOuts = res.data;
            })
            .catch(err => {
              console.log(err);
            });
          // get histories dept in
          axios
            .get("employees/histories/" + this.user.Id + "/dept/in")
            .then(res => {
              this.hisDeptIns = res.data;
            })
            .catch(err => {
              console.log(err);
            });
          // get histories dept out
          axios
            .get("employees/histories/" + this.user.Id + "/dept/out")
            .then(res => {
              this.hisDeptOuts = res.data;
            })
            .catch(err => {
              console.log(err);
            });
          //if (res.data == true) this.responeMessage = "Nạp tiền thành công!";
          //else this.responeMessage = "Không tìm thấy thông tin tài khoản!";
          //this.$refs["respone"].show();
        })
        .catch(err => {
          this.statusOk = false;
          console.log(err);
          this.responeMessage = "Không tìm thấy thông tin tài khoản!";
          this.$refs["respone"].show();
        });
    },
    canceled() {
      this.user.AccountNumber = "";
      this.user.Username = "";
      this.statusOk = false;
      this.send = this.valueType;
      this.payInfo = {};
      this.valueType = "";
      this.type = 1;
    }
  }
};
</script>
