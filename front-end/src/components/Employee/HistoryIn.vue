<template>
  <div class="HistoryIn">
    <b-row>
          <b-col sm="5" md="6" class="my-1">
            <b-form-group
              label="Hiển thị"
              label-cols-sm="6"
              label-cols-md="4"
              label-cols-lg="3"
              label-align-sm="right"
              label-size="sm"
              label-for="perPageSelect"
              class="mb-0"
            >
              <b-form-select v-model="perPage1" id="perPageSelect" size="sm" :options="pageOptions"></b-form-select>
            </b-form-group>
          </b-col>
          <b-col lg="6" class="my-1">
            <b-form-group
              label="Tìm kiếm"
              label-cols-sm="3"
              label-align-sm="right"
              label-size="sm"
              label-for="filterInput"
              class="mb-0"
            >
              <b-input-group size="sm">
                <b-form-input
                  v-model="filter"
                  type="search"
                  id="filterInput"
                  placeholder="Tìm kiếm"
                ></b-form-input>
                <b-input-group-append>
                  <b-button :disabled="!filter" @click="filter = ''">Xóa</b-button>
                </b-input-group-append>
              </b-input-group>
            </b-form-group>
          </b-col>
        </b-row>

        <!-- Main table element -->
        <b-table
          id="my-table1"
          show-empty
          small
          stacked="md"
          :items="items"
          :fields="fields"
          :current-page="currentPage1"
          :per-page="perPage1"
          :filter="filter"
          :filterIncludedFields="filterOn"
          :sort-by.sync="sortBy"
          :sort-desc.sync="sortDesc"
          :sort-direction="sortDirection"
          @filtered="onFiltered"
        >
          <template v-slot:cell(name)="row">{{ row.value.first }} {{ row.value.last }}</template>

          <template v-slot:cell(actions)="row">
            <b-button
              size="sm"
              @click="info(row.item, row.index, $event.target)"
              class="mr-1"
            ><b-icon icon="document-text"></b-icon></b-button>
          </template>
        </b-table>
        <b-row>
          <b-col sm="6" md="6" class="my-1">
            <div></div>
          </b-col>
          <b-col sm="6" md="6" class="my-1">
            <b-pagination
              aria-controls="my-table1"
              v-model="currentPage1"
              :total-rows="totalRows1"
              :per-page="perPage1"
              class="mt-4"
              align="right"
            ></b-pagination>
          </b-col>
        </b-row>

        <b-modal :id="infoModal.id" :title="infoModal.title" ok-only @hide="resetInfoModal">
          <p>Tên: <b>{{infoModal.content.AccountName}}</b></p>
          <p>Số tài khoản: <b>{{infoModal.content.AccountNumber}}</b></p>
          <p>Ngân hàng: <b>{{infoModal.content.BankName}}</b></p>
          <p>Số tiền chuyển: <b>{{infoModal.content.Money}}</b></p>
          <p>Nội dung: <b>{{infoModal.content.Description}}</b></p>
          <p>Ngày: <b>{{ infoModal.content.ConfirmTime | formatDate }}</b></p>
          <p v-if="infoModal.content.IsPayIn != null && infoModal.content.IsPayIn">Ghi chú: Nạp tiền từ nhân viên!</p>
        </b-modal>
  </div>
</template>

<script>
import moment from 'moment';

export default {
  name: "HistoryIn",
  props: {
    histories1: {
      type: Array
    }
  },
  watch: {
    histories1: function() {
      this.items = this.histories1
      this.totalRows1 = this.items.length;
    }
  },
  data() {
    return {
      passMessage: "",
      responeMessage: "",
      payInfo: {
        UserName: "",
        AccountNumber: "",
        Money: 0
      },
      valueType: "",
      type: 1,
      Types: [
        { value: 1, text: "Tên đăng nhập" },
        { value: 2, text: "Số tài khoản" }
      ],
      items: [],
      fields: [
        {
          key: "AccountNumber",
          label: "Tài khoản chuyển",
          sortable: true,
          sortDirection: "desc"
        },
        {
          key: "AccountName",
          label: "Tên",
          sortable: true,
          class: "text-center"
        },
        {
          key: "Money",
          label: "Số tiền",
          sortable: true,
          class: "text-center"
        },
        {
          key: "Description",
          label: "Nội dung",
          sortable: true,
          class: "text-center"
        },
        {
          key: "ConfirmTime",
          label: "Ngày",
          sortable: false,
          formatter: (value) => {
            return moment(String(value)).format('DD/MM/YYYY hh:mm:ss')
          },
          class: "text-center"
        },
        { key: "actions", label: "#", class: "text-center"}
      ],
      totalRows1: 1,
      currentPage1: 1,
      perPage1: 5,
      pageOptions: [1, 2, 5, 10, 15],
      sortBy: "",
      sortDesc: false,
      sortDirection: "asc",
      filter: null,
      filterOn: [],
      infoModal: {
        id: "info-modal-In",
        title: "",
        content: ""
      }
    };
  },
  computed: {
    sortOptions() {
      // Create an options list from our fields
      return this.fields
        .filter(f => f.sortable)
        .map(f => {
          return { text: f.label, value: f.key };
        });
    }
  },
  mounted() {
    // Set the initial number of items
    this.totalRows1 = this.items.length;
  },
  methods: {
    canceled() {
      this.payInfo = {};
      this.valueType = "";
      this.type = 1;
    },
    info(item, index, button) {
      this.infoModal.title = "Thông tin người gửi";
      this.infoModal.content = item;
      this.$root.$emit("bv::show::modal", this.infoModal.id, button);
    },
    resetInfoModal() {
      this.infoModal.title = "";
      this.infoModal.content = "";
    },
    onFiltered(filteredItems) {
      // Trigger pagination to update the number of buttons/pages due to filtering
      this.totalRows1 = filteredItems.length;
      this.currentPage1 = 1;
    }
  }
};
</script>
