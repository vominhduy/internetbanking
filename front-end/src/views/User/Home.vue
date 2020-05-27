<template>
  <div class="home">
    <img alt="Vue logo" src="../../assets/DDP_logo.png">
    <Hello msg="Chào mừng tới ngân hàng DDP"/>
  </div>
</template>

<script>
// @ is an alias to /src
import Hello from '@/components/Hello.vue'
import apiHelper from '../../helper/call_api'

export default {
  name: 'Home',
  components: {
    Hello
  },
  data() {
    return {
      IsClosedBankAccount: false
    }
  },
  mounted: function(){
    this.getLinkingAccounts();
  },
  methods: {
    getLinkingAccounts(){
    let me = this;
    apiHelper
      .call_api(`Users`, "get", '')
      .then(res => {
          if(res.data && res.data.CheckingAccount){
              me.IsClosedBankAccount = res.data.CheckingAccount.IsClosed;
              localStorage.setItem('IsClosedBank', me.IsClosedBankAccount);
          }
      })
      .catch(err => {
          console.error(err);
      });
    },
  },
}
</script>
