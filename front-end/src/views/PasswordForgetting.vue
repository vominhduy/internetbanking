<template>
  <div class="martop">
      <b-form @submit.stop.prevent="onSubmit">
        <b-form-group label-cols-sm="12" label-cols-md="4" label="Email phục hồi" label-for="value">
          <b-form-input
            id="email"
            name="email"
            type="email"
            v-model="user.Email"
            v-validate="'required|email'"
            :state="validateState('email')"
            aria-describedby="emailFeedback"
            placeholder="Email"
          ></b-form-input>
          <b-form-invalid-feedback id="emailFeedback">Không đúng định dạng email!</b-form-invalid-feedback>
        </b-form-group>
        <b-form-group>
          <b-row>
            <b-col>
              <b-button block @click.prevent="forgetPassword($event)" variant="success">Xác nhận</b-button>
            </b-col>
          </b-row>
        </b-form-group>
      </b-form>
      <b-link to="/">Đăng nhập</b-link>
    <b-modal ref="ConfirmForgetPassword" title="Xác nhận mã xác thực">
      <b-form @submit.stop.prevent="onSubmit">
        <b-form-group label-cols-sm="12" label-cols-md="4" label="Mã xác thực" label-for="otp">
          <b-form-input
            id="otp"
            name="otp"
            type="text"
            v-model="user.Otp"
            v-validate="{required:true}"
            :state="validateState('otp')"
            aria-describedby="otpFeedback"
          ></b-form-input>
          <b-form-invalid-feedback id="otpFeedback">Mã xác thực không thể trống!</b-form-invalid-feedback>
        </b-form-group>
        <b-form-group>
          <b-row>
            <b-col>
              <b-button
                block
                @click.prevent="ConfirmForgetPassword($event)"
                variant="success"
              >Xác nhận</b-button>
            </b-col>
            <b-col>
              <b-button block variant="danger" @click.prevent="canceled($event)">Hủy</b-button>
            </b-col>
          </b-row>
        </b-form-group>
      </b-form>
      <template v-slot:modal-footer>
        <div hidden></div>
      </template>
    </b-modal>
  </div>
</template>

<style scoped>
.invalid-feedback,
.invalid-message {
  width: 100%;
  margin-top: 0.25rem;
  font-size: 90%;
  color: #dc3545;
  font-weight: bold;
}
.martop {
  margin-top: 70px;
  padding: 10px;
}
</style>

<script>
import apiHelper from "../helper/call_api";
import { Validator } from "vee-validate";

export default {
  name: "PasswordForgetting",
  components: {  },
  data() {
    return {
      user: {
        Email: "",
        Otp: "",
        Id: ""
      }
    };
  },
  methods: {
    validateState(ref) {
      if (
        this.veeFields[ref] &&
        (this.veeFields[ref].dirty || this.veeFields[ref].validated)
      ) {
        return !this.veeErrors.has(ref);
      }
      return null;
    },
    forgetPassword(evt) {
      evt.preventDefault();
      // console.log('vali result',  this.validateState('email'));
      this.$validator.validateAll().then(result => {
        

        if (!result)
          return;
        apiHelper
        .call_api(`accounts/passwords/Forget`, "post", this.user)
        .then(res => {
          if (res.data != "") {
            this.makeToast(
              "success",
              "Đã gửi mail thành công. Vui lòng xác nhận!"
            );
            this.user.Id = res.data;
            
            this.$refs["ConfirmForgetPassword"].show();
          } else {
            this.makeToast("danger", "Có lỗi xảy ra!");
          }
        })
        .catch(error => {
          this.makeToast("danger", error.response.data);
        });
      })

      // var a = 0;
      // if (a == 0)
      // return;

      
    },
    ConfirmForgetPassword(evt) {
      evt.preventDefault();

      this.$validator.validateAll().then(result => {
        

        if (!result)
          return;

          apiHelper
        .call_api(`accounts/passwords/ConfirmForgetting`, "post", this.user)
        .then(res => {
          if (res.data == true) {
            this.makeToast(
              "success",
              "Đã tạo lại mật khẩu thành công. Vui lòng xác nhận!"
            );
            this.$refs["ConfirmForgetPassword"].hide();
          } else {
            this.makeToast("danger", "Có lỗi xảy ra!");
          }
          
        })
        .catch(error => {
          this.makeToast("danger", error.response.data);
          //this.$refs["ConfirmForgetPassword"].hide();
        });
      });

      
    },
    makeToast(variant = null, content = null) {
      this.$bvToast.toast(content, {
        title: "Thông báo!",
        autoHideDelay: 3000,
        variant: variant,
        solid: true,
        toaster: "b-toaster-bottom-right"
      });
    },
    canceled(evt) {
      this.$refs["ConfirmForgetPassword"].hide();
      // Reset our form values
      //this.user = {};

      this.$nextTick(() => {
      })
    }
  }
};
</script>