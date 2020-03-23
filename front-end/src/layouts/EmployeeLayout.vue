<template>
  <div>
    <NavBar v-on:openNav="openNav" />
    <div id="mySidenav" class="sidenav">
      <router-link to="/employee/create-user">Tạo tài khoản khách hàng</router-link>
      <router-link to="/employee/pay-in">
        Nạp tiền tài khoản khách hàng
        <b-icon icon="chevron-right"></b-icon>
      </router-link>
      <router-link to="/employee/histories">
        Xem lịch sử giao dịch
        <b-icon icon="chevron-right"></b-icon>
      </router-link>
    </div>
    <div id="main">
      <div
        style="table-layout: fixed;
    border-collapse: collapse;
    z-index: -1; margin-top:40px;"
      >
        <b-row>
          <b-col>
            <slot />
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
            <p>sdfsdf</p>
          </b-col>
        </b-row>
      </div>
    </div>
  </div>
</template>

<script>
import NavBar from "@/components/NavBar.vue";

export default {
  name: "AdminLayout",
  components: {
    NavBar
  },
  data() {
    return {
      isSidebar: false
    };
  },
  methods: {
    openNav() {
      if (!this.isSidebar) {
        document.getElementById("mySidenav").style.width = "250px";
        document.getElementById("main").style.marginLeft = "250px";
        this.isSidebar = true;
      } else {
        document.getElementById("mySidenav").style.width = "0";
        document.getElementById("main").style.marginLeft = "0";
        this.isSidebar = false;
      }
    },
    closeNav() {
      document.getElementById("mySidenav").style.width = "0";
      document.getElementById("main").style.marginLeft = "0";
    }
  },
  mounted() {
    console.log("sdfsdfdsfdsfdfvv");
    this.openNav();
  }
};
</script>
<style>
.sidenav {
  height: 100%;
  width: 0px;
  position: fixed;
  z-index: 1;
  top: 0;
  left: 0;
  background-color: #3c4b64;
  overflow-x: hidden;
  transition: 0.5s;
  padding-top: 16px;
  margin-top: 56px;
  margin-left: 0px;
}

.sidenav a {
  padding: 8px 8px 8px 32px;
  text-decoration: none;
  color: hsla(0, 0%, 100%, 0.8);
  display: block;
  transition: 0.3s;
  font-size: smaller;
}

.sidenav a:hover {
  color: white;
  background-color: lightslategray;
}

#main {
  transition: margin-left 0.5s;
  padding: 16px;
}

@media screen and (max-height: 450px) {
  .sidenav {
    padding-top: 16px;
  }
}
/* width */
.sidenav::-webkit-scrollbar {
  width: 8px;
  -ms-overflow-style: -ms-autohiding-scrollbar;
}

/* Track */
.sidenav::-webkit-scrollbar-track {
  border-radius: 8px;
}

/* Handle */
.sidenav::-webkit-scrollbar-thumb {
  background: #aaaaaa;
  border-radius: 8px;
}

/* Handle on hover */
.sidenav::-webkit-scrollbar-thumb:hover {
  background: #999999;
}
</style>