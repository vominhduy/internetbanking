<template>
  <div class="float-left">
      <b-list-group>
        <b-list-group-item
          :to="`/employee/create-user`"
          v-b-toggle.collapse-1
        >Tạo tài khoản khách hàng</b-list-group-item>
        <b-list-group-item
          :to="`/employee/pay-in`"
          v-b-toggle.collapse-2
        >Nạp tiền tài khoản khách hàng</b-list-group-item>
        <b-list-group-item :to="`/employee/histories`" v-b-toggle.collapse-3>
          Xem lịch sử giao dịch
        </b-list-group-item>
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