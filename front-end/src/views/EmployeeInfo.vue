<template>
  <div>
    <div v-if="empty" class="card-body">
      <p class="card-text">Không có dữ liệu.</p>
    </div>
    <div v-else class="card-body">
      {{info.data.email}}<br/>
      {{info.data.first_name}}<br/>
      {{info.data.last_name}}<br/>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: 'EmployeeInfo',

  data() {
    return {
      info: {},
      empty: false
    }
  },

  mounted(){
    this.fetchEmployee(this.$route.params.id);
  },

  watch: {
    $route(to){
      this.fetchEmployee(to.params.id);
    }
  },

  methods:{
    fetchEmployee(id){
      axios
        .get(`https://reqres.in/api/users/${id}`)
        .then(res => {
          this.info = res.data;
          this.empty = false;
        })
        .catch(err => {
          this.empty = true;
          console.log(err);
        })
    }
  }
}
</script>

<style>

</style>
