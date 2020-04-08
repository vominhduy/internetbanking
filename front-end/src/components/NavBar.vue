<template>
  <div>
    <b-navbar class="nav" toggleable="lg" type="light" variant="info" fixed="top">
      <div style="width: 234px; text-align: center;">
        <img style="width: 70px; cursor:pointer" src="../assets/DDP_logo.png" v-on:click="$emit('openHome')"/>
        <b-icon v-on:click="$emit('openNav')" icon="code" class="sidebar"></b-icon>
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
          <b-nav-item-dropdown text="Lang" right>
            <b-dropdown-item href="#">EN</b-dropdown-item>
            <b-dropdown-item href="#">ES</b-dropdown-item>
            <b-dropdown-item href="#">RU</b-dropdown-item>
            <b-dropdown-item href="#">FA</b-dropdown-item>
          </b-nav-item-dropdown>

          <b-nav-item-dropdown right>
            <!-- Using 'button-content' slot -->
            <template v-slot:button-content>
              <em>User</em>
            </template>
            <b-dropdown-item href="#">Profile</b-dropdown-item>
            <b-dropdown-item href="#" @click="logout">Sign Out</b-dropdown-item>
          </b-nav-item-dropdown>
        </b-navbar-nav>
      </b-collapse>
    </b-navbar>
  </div>
</template>

<script>
export default {
  name: "NavBar",
  methods: {
    logout() {
      this.$store.dispatch("destroyToken").then(res => {
        console.log(res);
        this.$router.push({ name: "Login" });
      });
    }
  }
};
</script>

<style scoped>
.sidebar {
  font-size: 35px;
  cursor: pointer;
  float: right;
  margin-top: 2px;
  color: #6a727a;
  transition: 0.3s;
}
.sidebar:hover {
  font-size: 35px;
  cursor: pointer;
  float: right;
  margin-top: 2px;
  color: rgba(0, 0, 0, 0.7);
}
.bg-info {
  background-color: whitesmoke !important;
}
.nav {
  border-bottom: solid 1px rgba(0, 0, 0, 0.5);
}
</style>