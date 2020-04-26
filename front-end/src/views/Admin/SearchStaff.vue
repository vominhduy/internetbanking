<template>
  <div>
   <b-form>
      <b-form-group label="Xóa / Sửa nhân viên" style="width: 30%;">
        <b-form-input
          type="text"
          required
          placeholder="Tên đăng nhập"
        >
        
        </b-form-input>
        <b-button type="submit" style="float:right" variant="primary">Tìm kiếm</b-button>
      </b-form-group>
    </b-form>
  </div>
</template>

<script>
export default {
  name: "SearchStaff",
  components: {}
};
</script>