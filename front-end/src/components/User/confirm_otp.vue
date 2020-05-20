<template>
  <div>
    <b-card class="bcard-shadow" v-if="show==false">
      <div class="header">
        <h4 class="title">
          <b-icon icon="chevron-right"></b-icon>&nbsp;Xác nhận
        </h4>
        <hr />
      </div>
      <div class="body">
        <b-form>
          <label>Mã xác nhận (OTP) đã được gửi đến email {{info_confirm_otp.email}} của quý khách.</label>
          <br />
          <br />
          <b-form-group
            label-cols-sm="12"
            label-cols-md="4"
            label="Nhập mã xác nhận"
            label-for="opt"
          >
            <b-form-input
              id="otp"
              name="otp"
              v-validate="{required:true}"
              :state="validateState('otp')"
              aria-describedby="otpFeedback"
              v-model="otp"
            ></b-form-input>
            <b-form-invalid-feedback id="otpFeedback">Mã xác nhận không được để trống!</b-form-invalid-feedback>
          </b-form-group>

          <b-form-group>
            <b-row>
              <b-col>
                <b-button
                  class="mb-2 float-right"
                  variant="success"
                  @click.prevent="changeComponet"
                >
                  Xác nhận
                  <b-icon icon="chevron-right"></b-icon>
                </b-button>
              </b-col>
            </b-row>
            <!--
            <b-label for="usr">{{info_confirm_otp.full_name}}:</b-label>
            <b-label>{{info_confirm_otp.email}}:</b-label>
            <b-label>{{info_confirm_otp.transaction_id}}:</b-label>
            -->
          </b-form-group>
        </b-form>
      </div>
    </b-card>

    <component v-if="show==true" v-bind:is="component" v-bind:info_transfer="info_transfer" />
  </div>
</template>

<style scoped>
.bcard-shadow {
  margin-top: 15px;
  box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
}
</style>

<script>
import NavBar from "@/components/User/confirm_otp.vue";
import Categories from "@/components/User/transfer_money_info.vue";
import axios from "axios";
import helper from "../../helper/call_api.js";

export default {
  name: "confirm_otp",
  props: ["info_confirm_otp"],
  components: {
    NavBar,
    Categories
  },
  data() {
    return {
      show: false,
      component: "Categories",
      info_transfer: {
        is_success: false,
        message: "",
        destination_account_number: "",
        amount: 0
      },
      otp: "",
      transaction_id: ""
    };
  },
  //this.$props.info_confirm_otp.transaction_id
  methods: {
    changeComponet() {
      // validate
      this.$validator.validate('otp').then(result => {
        if (!result) {
          return;
        }

        //
        helper
          .call_api(
            "users/confirmtransfer/" +
              this.$props.info_confirm_otp.transaction_id +
              "/?otp=" +
              this.otp,
            "post",
            {}
          )
          .then(res => {
            console.log("s");
            console.log(res);
            if (res.status == "200") {
              this.info_transfer.is_success = this.data;
              this.info_transfer.message =
                res.data == true
                  ? "Quý khách đã chuyển tiền thành công!"
                  : "Chuyển tiền thất bại!. Quý khách vui lòng kiểm tra lại mã xác nhận.";
              this.info_transfer.destination_account_number = this.$props.info_confirm_otp.destination_account_number;
              this.info_transfer.amount = this.$props.info_confirm_otp.amount;
              this.show = true;
            } else {
              this.info_transfer.message =
                res.data == true
                  ? "Quý khách đã chuyển tiền thành công!"
                  : "Chuyển tiền thất bại!. Quý khách vui lòng kiểm tra lại mã xác nhận.";
            }
          })
          .catch(err => {
            console.log("e");
            console.log(err);
            this.info_transfer.message = "Error";
            this.show = true;
          });
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
</style>