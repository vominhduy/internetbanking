<template>
  <div>
    <b-form @submit.prevent="login" @reset="onReset" v-if="show">
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
        login(){
          this.$store.dispatch('retrieveLogin', {
            username: this.form.username,
            password: this.form.password
          })
          .then(res => {
            if(res){
              if(res.role === "Employee"){
                this.$router.push('/employee');
              }
              else {
                this.$router.push('/user');
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
      }
  }
</script>