<template>
  <div>
    <b-form @submit.prevent="login" @reset="onReset" v-if="show">
      <div id="ServerFeedback" class="invalid-message">{{form.serverErrorMessage}}</div>
      <b-form-group label="Username:">
        <b-form-input
          name="username"
          v-model="form.username"
          type="text"
          v-validate="{required:true}"
          :state="validateState('username')"
          placeholder="Tài khoản đăng nhập"
        ></b-form-input>
        <b-form-invalid-feedback id="UsernameFeedback">Tài khoản đăng nhập không được để trống!</b-form-invalid-feedback>
      </b-form-group>

      <b-form-group label="Password:">
        <b-form-input
          name="password"
          v-model="form.password"
          type="password"
          v-validate="{required:true}"
          :state="validateState('password')"
          placeholder="Mật khẩu"
        ></b-form-input>
        <b-form-invalid-feedback id="PasswordFeedback">Mật khẩu không được để trống!</b-form-invalid-feedback>
        <br />
        <vue-recaptcha
          @verify="verifyRecaptcha"
          sitekey="6LcL7eQUAAAAANOMygbJIXb8yVFQc9vc-vUawUak"
          :loadRecaptchaScript="true"
        ></vue-recaptcha>
        <div id="CaptchaFeedback" class="invalid-message">{{form.recaptchaVerifiedMessage}}</div>
      </b-form-group>
      <b-form-group>
        <b-link href="#" @click.prevent="showForgetPassword">Quên mật khẩu?</b-link>
      </b-form-group>
      <b-button type="submit" variant="primary">Submit</b-button>
      <b-button type="reset" variant="danger">Reset</b-button>
    </b-form>
    <b-modal ref="ForgetPassword" title="Quên mật khẩu">
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
          ></b-form-input>
          <b-form-invalid-feedback id="emailFeedback">Không đúng định dạng email!</b-form-invalid-feedback>
        </b-form-group>
        <b-form-group>
          <b-row>
            <b-col>
              <b-button block @click.prevent="forgetPassword($event)" variant="success">Xác nhận</b-button>
            </b-col>
            <b-col>
              <b-button block variant="danger" @click.prevent="canceled">Hủy</b-button>
            </b-col>
          </b-row>
        </b-form-group>
      </b-form>
      <template v-slot:modal-footer>
        <div hidden></div>
      </template>
    </b-modal>
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
              <b-button block variant="danger" @click.prevent="canceled">Hủy</b-button>
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
  .invalid-feedback, .invalid-message {
  width: 100%;
  margin-top: 0.25rem;
  font-size: 90%;
  color: #dc3545;
  font-weight: bold;
}
</style>

<script>
import VueRecaptcha from "vue-recaptcha";
import apiHelper from "../../helper/call_api";
import { Validator } from "vee-validate";

export default {
  name: "Login",
  components: { VueRecaptcha },
  data() {
    return {
      form: {
        username: "",
        password: "",
        recaptchaVerified: false,
        recaptchaVerifiedMessage: "",
        serverErrorMessage:'',
      },
      show: true,
      forgetPasswordEmail: "",
      user: {
        Email: "",
        Otp: "",
        Id: ""
      }
    };
  },
  methods: {
    login() {
      if (!this.form.recaptchaVerified) {
        this.form.recaptchaVerifiedMessage = "Vui lòng xác thực captcha";
        return;
      }
      // if (!this.veeErrors.has("password")) {
      //   return;
      // }
      // if (!this.veeErrors.has("username")) {
      //   return;
      // }
      this.$store
        .dispatch("retrieveLogin", {
          username: this.form.username,
          password: this.form.password
        })
        .then(res => {
          if (res) {
            if (res.role === "Employee") {
              this.$router.push({ name: "EmployeeHome" });
            } else if (res.role === "User") {
              this.$router.push({ name: "UserHome" });
            } else if (res.role === "Admin") {
              this.$router.push({ name: "AdminHome" });
            }
          }
        })
        .catch((err) => {
          if(err.response.data){
            this.form.serverErrorMessage = err.response.data;
          }
        });
    },
    onReset(evt) {
      evt.preventDefault();
      // Reset our form values
      this.form.username = "";
      this.form.password = "";
      // Trick to reset/clear native browser form validation state
      this.show = false;
      this.$nextTick(() => {
        this.show = true;
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
    },
    verifyRecaptcha(response) {
      this.form.recaptchaVerified = true;
      this.form.recaptchaVerifiedMessage = "";
    },
    showForgetPassword() {
      this.$refs["ForgetPassword"].show();
    },
    forgetPassword(evt) {
      evt.preventDefault();
      // console.log('vali result',  this.validateState('email'));
      // if (this.veeErrors.has("email")) {
      //   console.log('email');
      //   return;
      // }

      // var a = 0;
      // if (a == 0)
      // return;

      apiHelper
        .call_api(`accounts/passwords/Forget`, "post", this.user)
        .then(res => {
          if (res.data != "") {
            this.makeToast(
              "success",
              "Đã gửi mail thành công. Vui lòng xác nhận!"
            );
            this.user.Id = res.data
            this.$refs["ForgetPassword"].hide();
            this.$refs["ConfirmForgetPassword"].show();
          } else {
            this.makeToast("danger", "Có lỗi xảy ra!");
          }
        })
        .catch(error => {
          this.makeToast("danger", error.response.data);
        });
    },
    ConfirmForgetPassword(evt) {
      evt.preventDefault();

      apiHelper
        .call_api(`accounts/passwords/ConfirmForgetting`, "post", this.user)
        .then(res => {
          if (res.data == true) {
            this.makeToast(
              "success",
              "Đã tạo lại mật khẩu thành công. Vui lòng xác nhận!"
            );

            setTimeout(() => {
              //this.$router.push({ name: "Login" });
            }, 2000);
          } else {
            this.makeToast("danger", "Có lỗi xảy ra!");
          }
        })
        .catch(error => {
          this.makeToast("danger", error.response.data);
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
    }
  }
};
</script>