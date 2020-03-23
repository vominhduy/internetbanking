<template>
  <div>
    <component :is="this.$route.meta.layout || 'div'">
      <router-view />
    </component>
  </div>
</template>

<script>
  export default {
    name: "App",
  }
</script>

<style lang="scss" scoped>
@import "@coreui/coreui/scss/coreui";
</style>