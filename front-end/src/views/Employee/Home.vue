<template>
  <b-card style="margin-top: 15px; box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);">
    <h2>BẢNG ĐIỀU KHIỂN</h2>
    <p>Chào mừng bạn đến với hệ thống quản trị khách hàng - {{ new Date() | formatDate }}</p>
    <b-row>
      <b-col>
        <b-button block variant="outline-success" size="lg" to="/employee/create-user"><b-icon icon="person"></b-icon> Tạo tài khoản</b-button>
      </b-col>
      <b-col>
        <b-button block size="lg" variant="outline-secondary" to="/employee/pay-in"><b-icon icon="graph-up"></b-icon> Nạp tiền</b-button>
      </b-col>
      <b-col>
        <b-button block variant="outline-primary" size="lg" to="/employee/histories"><b-icon icon="alarm"></b-icon> Xem lịch sử</b-button>
      </b-col>
    </b-row>
  </b-card>
</template>

<script>
export default {
  name: "EmployeeHome",
  components: {}
};
</script>
