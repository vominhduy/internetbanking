<template>
  <div>
    <b-navbar class="nav" toggleable="lg" type="light" variant="info" fixed="top">
      <div style="width: 234px; text-align: center;">
        <img
          style="width: 70px; cursor:pointer"
          src="../assets/DDP_logo.png"
          v-on:click="$emit('openHome')"
        />
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
            <b-dropdown-item href="#" @click="showChangePassworrd">Đổi mật khẩu</b-dropdown-item>
            <b-dropdown-item href="#" @click="logout">Đăng xuất</b-dropdown-item>
          </b-nav-item-dropdown>
        </b-navbar-nav>
      </b-collapse>
    </b-navbar>
    <b-modal ref="ChangePassword" title="Đổi mật khẩu">
      <b-form @submit.stop.prevent="onSubmit">
        <b-form-group
          label-cols-sm="12"
          label-cols-md="4"
          label="Mật khẩu hiện tại"
          label-for="value"
        >
          <b-form-input
            id="oldPassword"
            name="oldPassword"
            type="password"
            v-model="user.OldPassword"
            v-validate="{required:true}"
            :state="validateState('oldPassword')"
            aria-describedby="valuefeedback"
          ></b-form-input>
          <b-form-invalid-feedback id="valuefeedback">Mật khẩu hiện tại không được để trống!</b-form-invalid-feedback>
        </b-form-group>
        <b-form-group
          label-cols-sm="12"
          label-cols-md="4"
          label="Mật khẩu mới"
          label-for="newPassword1"
        >
          <b-form-input
            id="newPassword1"
            name="newPassword1"
            type="password"
            v-model="user.NewPassword"
            v-validate="{required:true}"
            :state="validateState('newPassword1')"
            aria-describedby="newPassword1Feedback"
            ref="newPassword1"
          ></b-form-input>
          <b-form-invalid-feedback id="newPassword1Feedback">Mật khẩu mới không được để trống!</b-form-invalid-feedback>
        </b-form-group>
        <b-form-group
          label-cols-sm="12"
          label-cols-md="4"
          label="Nhập lại mật khẩu mới"
          label-for="newPassword2"
        >
          <b-form-input
            id="newPassword2"
            name="newPassword2"
            type="password"
            v-validate="'confirmed:newPassword1'"
            :state="validateState('newPassword2')"
            aria-describedby="newPassword2Feedback"
            data-vv-as="newPassword1"
          ></b-form-input>
          <b-form-invalid-feedback id="newPassword2Feedback">Mật khẩu vừa nhập không trùng!1</b-form-invalid-feedback>
        </b-form-group>
        <b-form-group>
          <b-row>
            <b-col>
              <b-button block type="submit" variant="success">Xác nhận</b-button>
            </b-col>
            <b-col>
              <b-button block variant="danger" @click.prevent="canceled">Hủy</b-button>
            </b-col>
          </b-row>
        </b-form-group>
      </b-form>
      <template v-slot:modal-footer>
        <div hidden></div>
      </template>
    </b-modal>
  </div>
</template>

<script>
import apiHelper from "../helper/call_api";
import { Validator } from "vee-validate";

export default {
  name: "NavBar",
  data() {
    return {
      user: {
        OldPassword: "",
        NewPassword: ""
      }
    };
  },
  methods: {
    logout() {
      this.$store.dispatch("destroyToken").then(res => {
        console.log(res);
        this.$router.push({ name: "Login" });
      });
    },
    showChangePassworrd() {
      this.user = {};
      this.$refs["ChangePassword"].show();
    },
    resetPass() {
      this.makeToast("success", "dd");

      var a = 0;
      if (a == 0) return;
      this.$store.dispatch("destroyToken").then(res => {
        console.log(res);
        this.$router.push({ name: "Login" });

        // reset
        apiHelper
          .call_api(`accounts/passwords/change`, "post", this.user)
          .then(res => {})
          .catch(err => {
            console.log(err);
          });
      });
    },
    makeToast(variant = null, content = null) {
      this.$bvToast.toast(content, {
        title: "Thông báo!",
        autoHideDelay: 3000,
        variant: variant,
        solid: true,
        toaster: "b-toaster-bottom-right"
      });
    },
    validateState(ref) {
      if (
        this.veeFields[ref] &&
        (this.veeFields[ref].dirty || this.veeFields[ref].validated)
      ) {
        return !this.veeErrors.has(ref);
      }
      return null;
    },
    onSubmit(evt) {
      evt.preventDefault();
      this.$validator.validateAll().then(result => {
        if (!result) {
          return;
        }

        apiHelper
          .call_api(`accounts/passwords/change`, "post", this.user)
          .then(res => {
            if (res.data == true) {
              this.makeToast("success", "Đổi mật khẩu thành công");

              setTimeout(() => {
                this.$router.push({ name: "Login" });
              }, 2000);
            } else {
              this.makeToast("danger", "Có lỗi xảy ra!");
            }
          })
          .catch(error => {
            this.makeToast("danger", error.response.data);
          });
      });
    },
    canceled(evt) {
      evt.preventDefault();
      // Reset our form values

      this.$nextTick(() => {});
      //this.user = {};
      this.$refs["ChangePassword"].hide();
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