<template>
  <b-card class="bcard-shadow">
    <h1>Tạo nhắc nợ</h1>
    <b-form @submit.prevent="onSubmit" @reset="onReset" v-if="show">
      <b-form-group
        label="Số tài khoản:"
        description=""
      >
        <b-form-input
          name="accountNumber"
          type="text"
          v-model="form.accountNumber"
          v-validate="{required:true}"
          :state="validateState('accountNumber')"
          placeholder="Điền số tài khoản"
        ></b-form-input>
        <b-form-invalid-feedback id="AccountNumberFeedback">Số tài khoản không được để trống!</b-form-invalid-feedback>
      </b-form-group>

      <b-form-group id="input-group-2" label="Số tiền:" label-for="input-2">
        <b-form-input
          name="money"
          v-model="form.money"
          v-validate="{required:true}"
          :state="validateState('money')"
          placeholder="Nhập số tiền"
        ></b-form-input>
        <b-form-invalid-feedback id="MoneyFeedback">Số tiền không được để trống!</b-form-invalid-feedback>
      </b-form-group>

      <b-form-group
      class="mb-0"
      label="Nội dung nhắc nợ"
      label-for="textarea-description"
      description="">
        <b-form-textarea
          name="description"
          id="textarea-description"
          placeholder="Nhập nội dung nhắc nợ"
          v-model="form.description"
          v-validate="{required:true}"
          :state="validateState('description')"
        ></b-form-textarea>
        <b-form-invalid-feedback id="DescriptionFeedback">Nội dung nhắc nợ không được để trống!</b-form-invalid-feedback>
      </b-form-group>
      <br/>
        <b-button type="submit" variant="primary">Gửi nhắc nợ</b-button>
        <b-button type="reset" variant="danger">Hủy</b-button>
    </b-form>
      
  </b-card>
</template>

<script>
import apiHelper from '../../helper/call_api'
import utilsHelper from '../../helper/helper'
import { Validator } from "vee-validate"

export default {
  name: "Deptreminders",
  components: {
  },
  data() {
    return {
      form: {
        accountNumber: "",
        money: "",
        description: "",
      },
      show: true,
    };
  },
  mounted: function() {
    
  },

  methods: {
    onSubmit(evt) {
        evt.preventDefault()
        
        this.$validator.validateAll().then(result => {
          if (!result) {
            return;
          }
          let me = this;
          apiHelper
            .call_api(`deptreminders`, "post", {
              RecipientAccountNumber: this.form.accountNumber,
              Money: this.form.money,
              Description: this.form.description
            })
            .then(res => {
              if(res.status == 204){
                  utilsHelper.showErrorMsg(me, 'Thông tin tài khoản không hợp lệ!');
                  return;
              }
              utilsHelper.showSuccessfullMsg(me, 'Gửi nhắc nợ thành công!');
            })
            .catch(err => {
              console.error(err);
            });
        });
    },
    onReset(evt) {
      evt.preventDefault()
      // Reset our form values
      this.form.accountNumber = ''
      this.form.money = ''
      this.form.description = null
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

<style scoped>
.bcard-shadow {
  margin-top: 15px;
  box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
}
</style>