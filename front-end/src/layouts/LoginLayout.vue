<template>
  <div>
    <b-navbar class="nav" toggleable="lg" type="light" variant="info" fixed="top">
      <div style="width: 234px; text-align: center;">
        <img
          style="width: 70px; cursor:pointer"
          src="../assets/DDP_logo.png"
          @click.prevent="openHome"
        />
      </div>
      <b-navbar-brand href="#"></b-navbar-brand>

      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>
    </b-navbar>
    <b-row>
        <b-col sm="6">
          <img
          style="margin-top: 56px; width: 100%; cursor:pointer"
          src="../assets/home.png"
        />
        </b-col>
        <b-col sm="6">
          <slot />
        </b-col>
      </b-row>
  </div>
</template>

<script>
export default {
  name: "LoginLayout",
  methods: {
    openHome() {
      this.$router.push({ name: 'Login' });
    }
  }
};
</script>
<style scoped>
.nav {
  border-bottom: solid 1px rgba(0, 0, 0, 0.5);
  background-color: whitesmoke !important
}
</style>