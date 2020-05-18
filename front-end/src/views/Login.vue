<template>
  <div>
    <b-form @submit="onSubmit" @reset="onReset" v-if="show" style="padding: 10px;">
      <b-form-group label="Username:">
        <b-form-input
          v-model="form.username"
          type="text"
          required
          placeholder="Enter username"
        ></b-form-input>
      </b-form-group>

      <b-form-group label="Password:">
        <b-form-input
          v-model="form.password"
          type="password"
          required
          placeholder="Enter password"
        ></b-form-input>
      </b-form-group>

      <b-button type="submit" variant="primary">Submit</b-button>
      <b-button type="reset" variant="danger">Reset</b-button>
    </b-form>
  </div>
</template>

<script>
import axios from "axios";

  export default {
    name:'Login',
      data() {
        return {
            form: {
            username: '',
            password: '',
            },
            show: true
        }
      },
      methods: {
        onSubmit(evt) {
            evt.preventDefault();
            axios
              .post(`accounts/login`,{
                Username: this.form.username,
                Password: this.form.password
              })
              .then(res => {
                let token = res.data.AccessToken;
                let userInfo = this.parseJwt(token);
                if(userInfo.role === "Employee"){
                  this.$router.push('/employee');
                }
                
                console.log(res);
              })
              .catch(err => {
                this.empty = true;
                console.log(err);
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
        parseJwt (token) {
            var base64Url = token.split('.')[1];
            var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            var jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
                return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            }).join(''));

            return JSON.parse(jsonPayload);
        }
      }
  }
</script>