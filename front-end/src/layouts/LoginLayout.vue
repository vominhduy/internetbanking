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

      <b-collapse id="nav-collapse" is-nav>
        <!-- Right aligned nav items -->
        <b-navbar-nav class="ml-auto">
          <b-nav-form>
            <b-form-input size="sm" class="mr-sm-2" placeholder="Search"></b-form-input>
            <b-button size="sm" class="my-2 my-sm-0" type="submit">Search</b-button>
          </b-nav-form>

          <b-nav-item-dropdown text="Lang" right>
            <b-dropdown-item href="#">EN</b-dropdown-item>
            <b-dropdown-item href="#">ES</b-dropdown-item>
            <b-dropdown-item href="#">RU</b-dropdown-item>
            <b-dropdown-item href="#">FA</b-dropdown-item>
          </b-nav-item-dropdown>
        </b-navbar-nav>
      </b-collapse>
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
}
</style>