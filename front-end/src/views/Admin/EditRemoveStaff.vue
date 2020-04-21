<template>
  <div>
   <b-form>
     <b-form-group label="Xóa sửa nhân viên" style="width: 30%;">
        <b-form-input
          type="text"
          required
          placeholder="Tên đăng nhập"
        >
        </b-form-input>
        <b-button type="submit" style="float:right" variant="primary">Tìm kiếm</b-button>
      </b-form-group>
      <b-form-group label="Sửa xóa nhân viên">
        <b-form-input
          type="text"
          required
          placeholder="Tên"
        ></b-form-input>
      </b-form-group>
      <b-form-group label="Họ">
        <b-form-input
          type="text"
          required
          placeholder="Họ"
        ></b-form-input>
      </b-form-group>
      <b-form-group label="Email">
        <b-form-input
          type="text"
          required
          placeholder="Email"
        ></b-form-input>
      </b-form-group>

      <b-form-group label="Số điện thoại">
        <b-form-input
          type="text"
          required
          placeholder="Số điện thoại"
        ></b-form-input>
      </b-form-group>

      <b-button type="Cancel" style="margin:27px;" variant="primary">Hủy</b-button>
      <b-button type="Delete" style="margin:27px;" variant="primary">Xóa</b-button>
      <b-button type="Edit" style="margin:27px;" variant="primary">Edit</b-button>
    </b-form>
  </div>
</template>

<script>
export default {
  name: "AdminHome",
  components: {}
};
</script>
