<template>
  <div>
    <b-card no-body title="Title" header-tag="header" footer-tag="footer">
      <template v-slot:header>
        <h6 class="mb-0">Chức năng</h6>
      </template>
      <b-list-group>
        <b-list-group-item
          :to="`/employee/create-user`"
          v-b-toggle.collapse-1
        >Tạo tài khoản khách hàng</b-list-group-item>
        <b-list-group-item
          :to="`/employee/pay-in`"
          v-b-toggle.collapse-2
        >Nạp tiền tài khoản khách hàng</b-list-group-item>
        <b-list-group-item :to="`#`" v-b-toggle.collapse-3>
          Xem lịch sử giao dịch
          <b-icon-chevron-down></b-icon-chevron-down>
        </b-list-group-item>
        <b-collapse id="collapse-3" accordion="my-accordion" style="font-size: small">
          <b-list-group>
            <b-list-group-item :to="`/employee/historys/payin`">
              <b-icon-chevron-right></b-icon-chevron-right>Giao dịch nhận tiền
            </b-list-group-item>
            <b-list-group-item :to="`/employee/historys/payout`">
              <b-icon-chevron-right></b-icon-chevron-right>Giao dịch chuyển tiền
            </b-list-group-item>
            <b-list-group-item :to="`/employee/historys/deptin`">
              <b-icon-chevron-right></b-icon-chevron-right>Thanh toán nhắc nợ - nhận
            </b-list-group-item>
            <b-list-group-item :to="`/employee/historys/deptout`">
              <b-icon-chevron-right></b-icon-chevron-right>Thanh toán nhắc nợ - trả
            </b-list-group-item>
          </b-list-group>
        </b-collapse>
      </b-list-group>
    </b-card>
    <b-collapse id="collapse-1" accordion="my-accordion"></b-collapse>
    <b-collapse id="collapse-2" accordion="my-accordion"></b-collapse>
  </div>
</template>

<script>
export default {
  name: "SideBar"
};
</script>

<style scoped>
</style>