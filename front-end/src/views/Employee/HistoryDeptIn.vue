<template>
  <div class="HistoryDeptIn">
    <h1>
      Lịch sử thanh toán nhắc nợ - nhận
    </h1>
    <b-form @submit.stop.prevent="onSubmit">
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Loại" label-for="type">
        <b-form-select id="type" v-model="type" :options="Types"></b-form-select>
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Tên đăng nhập/Số tài khoản" label-for="value">
        <b-form-input id="value" v-model="valueType"></b-form-input>
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Số tiền" label-for="money">
        <b-form-input id="money" :type="'number'" v-model="payInfo.Money"></b-form-input>
      </b-form-group>
      <b-form-group>
        <b-row>
          <b-col>
            <b-button block type="submit" variant="success">Nạp tiền</b-button>
          </b-col>
          <b-col>
            <b-button block variant="danger" @click.prevent="canceled">Hủy</b-button>
          </b-col>
        </b-row>
      </b-form-group>
    </b-form>
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

export default {
  name: "HistoryDeptIn",
  data() {
    return {
      responeMessage: '',
      payInfo: {
        UserName: '',
        AccountNumber: '',
        Money: 0,
      },
      valueType: '',
      type: 1,
      Types: [
        { value: 1, text: "Tên đăng nhập" },
        { value: 2, text: "Số tài khoản" }
      ]
    };
  },
  methods: {
    onSubmit(evt) {
      evt.preventDefault();

      if (this.type == 1)
        this.payInfo.UserName = this.valueType;
      else
        this.payInfo.AccountNumber = this.valueType;

      axios
        .post(`employees/Users/payin`, this.payInfo)
        .then(res => {
          if (res.data == true)
            this.responeMessage = 'Nạp tiền thành công!';
          else
            this.responeMessage = 'Nạp tiền thất bại!';
          this.$refs["respone"].show();
        })
        .catch(err => {
          console.log(err);
        });
    },
    canceled() {
      this.payInfo = {};
      this.valueType = '';
      this.type = 1;
    }
  }
};
</script>
