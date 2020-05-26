<template>
  <b-card class="bcard-shadow">
    <h1>Nạp tiền vào tài khoản khách hàng</h1>
    <b-form @submit.stop.prevent="onSubmit" v-if="show">
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Loại" label-for="type">
        <b-form-select id="type" v-model="type" :options="Types"></b-form-select>
      </b-form-group>
      <b-form-group
        label-cols-sm="12"
        label-cols-md="4"
        label="Tên đăng nhập/Số tài khoản"
        label-for="value"
      >
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
      </b-form-group>
      <b-form-group label-cols-sm="12" label-cols-md="4" label="Số tiền (ĐV: nghìn đồng)" label-for="money">
        <b-form-input id="money" name="money" :type="'number'" v-model="payInfo.Money" v-validate="'required|between: 1000,50000000'" :state="validateState('money')"
          aria-describedby="moneyfeedback"></b-form-input>
        <b-form-invalid-feedback
          id="moneyfeedback"
        >Số phải phải nằm trong khoảng [1,000 - 50,000,000]!</b-form-invalid-feedback>
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
import { Validator } from 'vee-validate';


export default {
  name: "PayIn",
  data() {
    return {
      responeMessage: "",
      payInfo: {
        UserName: "",
        AccountNumber: "",
        Money: 0
      },
      valueType: "",
      type: 1,
      Types: [
        { value: 1, text: "Tên đăng nhập" },
        { value: 2, text: "Số tài khoản" }
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
        if (this.type == 1) this.payInfo.UserName = this.valueType;
        else this.payInfo.AccountNumber = this.valueType;

        apiHelper
          .call_api(`employees/Users/payin`, "post", this.payInfo)
          .then(res => {
            if (res.data == true) {
              this.responeMessage = "Nạp tiền thành công!";
              this.makeToast('success', "Nạp tiền thành công!");
            }
            else {
              this.responeMessage = "Nạp tiền thất bại!";
              this.makeToast('danger', "Nạp tiền thất bại!");
            }
            //this.$refs["respone"].show();
          })
          .catch(err => {
            console.error(err);
          });
      });
    },
    canceled(evt) {
      evt.preventDefault()
      // Reset our form values
      this.payInfo = {};
      this.valueType = "";
      this.type = 1;
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
    makeToast(variant = null, content = null) {
      this.$bvToast.toast(content, {
        title: "Thông báo!",
        autoHideDelay: 3000,
        variant: variant,
        solid: true,
        toaster: "b-toaster-bottom-right"
      });
    }
  }
};
</script>
