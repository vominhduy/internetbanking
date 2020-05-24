<template>
  <div class="home">
    <img alt="Vue logo" src="../../assets/DDP_logo.png">
    <Hello msg="Chào mừng tới ngân hàng DDP"/>
  </div>
</template>

<script>
// @ is an alias to /src
import Hello from '@/components/Hello.vue'

export default {
  name: 'Home',
  components: {
    Hello
  }
}
</script>
