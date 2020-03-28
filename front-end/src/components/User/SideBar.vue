<template>
  <div class="float-left">
    <b-list-group>
      <b-list-group-item :to="`/employee/`">Danh sách tài khoản</b-list-group-item>
      <b-list-group-item :to="`/employee/`">Thiết lập danh bạ</b-list-group-item>
      <b-list-group-item :to="`/user/remittance`">Chuyển khoản</b-list-group-item>
      <b-list-group-item :to="`/employee/`">Quản lý nhắc nợ</b-list-group-item>
      <b-list-group-item :to="`/employee/`">Xem lịch sử giao dịch</b-list-group-item>
      <b-list-group-item :to="`/employee/`">Đổi mật khẩu</b-list-group-item>
    </b-list-group>
  </div>
</template>

<script>
export default {
  name: "SideBar"
};
</script>

<style scoped>
</style>