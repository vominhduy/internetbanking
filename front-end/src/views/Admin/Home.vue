<template>
  <div class="home">
    <img alt="Vue logo" src="../../assets/logo.png">
    <Hello msg="Chào mừng tới ngân hàng DDP.js App"/>
  </div>
</template>

<script>
// @ is an alias to /src
import Hello from '@/components/Hello.vue'

export default {
  name: 'AdminHome',
  components: {
    Hello
  }
}
</script>
