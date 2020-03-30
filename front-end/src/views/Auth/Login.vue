<template>
  <div>
    <b-form @submit.prevent="login" @reset="onReset" v-if="show">
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
        <br>
        <vue-recaptcha @verify="verifyRecaptcha" sitekey="6LcL7eQUAAAAANOMygbJIXb8yVFQc9vc-vUawUak" :loadRecaptchaScript="true">
        </vue-recaptcha>
        <div id="CaptchaFeedback" class="invalid-captcha">{{form.recaptchaVerifiedMessage}}</div>
      </b-form-group>

      <b-button type="submit" variant="primary">Submit</b-button>
      <b-button type="reset" variant="danger">Reset</b-button>
    </b-form>
  </div>
</template>

<style scoped>
  .invalid-captcha {
    width: 100%;
    margin-top: .25rem;
    font-size: 80%;
    color: #dc3545;
  }
</style>

<script>
import VueRecaptcha from 'vue-recaptcha';

  export default {
    name:'Login',
    components: { VueRecaptcha },
      data() {
        return {
            form: {
              username: '',
              password: '',
              recaptchaVerified: false,
              recaptchaVerifiedMessage: '',
            },
            show: true
        }
      },
      methods: {
        login(){
          if(!this.form.recaptchaVerified){
            this.form.recaptchaVerifiedMessage = 'Vui lòng xác thực captcha';
            return;
          }
          this.$store.dispatch('retrieveLogin', {
            username: this.form.username,
            password: this.form.password
          })
          .then(res => {
            if(res){
              if(res.role === "Employee"){
                this.$router.push({ name: 'EmployeeHome' });
              }
              else if (res.role === "User")  {
                this.$router.push({ name: 'UserHome' });
              }
              else if (res.role === "Admin")  {
                this.$router.push({ name: 'AdminHome' });
              }
            }
          })
        },
        onReset(evt) {
            evt.preventDefault()
            // Reset our form values
            this.form.username = '';
            this.form.password = '';
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
        verifyRecaptcha(response){
          this.form.recaptchaVerified = true;
          this.form.recaptchaVerifiedMessage = '';
        }
      }
  }
</script>