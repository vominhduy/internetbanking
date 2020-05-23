<template>
<b-card class="bcard-shadow">
    <h1>Lịch sử Đối Soát IN</h1>
    <b-form @submit.stop.prevent="onSubmit">
        <b-form-group label-cols-sm="12" label-cols-md="4" label="Ngày bắt đầu ">
            <!-- <b-form-input id="value" name="value" v-model="valueType" v-validate="{required:true}" :state="validateState('value')" aria-describedby="valuefeedback"></b-form-input> -->
            <datetime format="YYYY-MM-DD" width="300px" v-model="fromDate"></datetime>
        </b-form-group>
        <b-form-group label-cols-sm="12" label-cols-md="4" label="Ngày kết thúc ">
            <!-- <b-form-input id="value" name="value" v-model="valueType" v-validate="{required:true}" :state="validateState('value')" aria-describedby="valuefeedback"></b-form-input> -->
            <datetime format="YYYY-MM-DD" width="300px" v-model="toDate"></datetime>
        </b-form-group>
        <b-form-group>
            <b-row>
                <b-col>
                    <b-button block type="submit" variant="success">Tra cứu</b-button>
                </b-col>
                <b-col>
                    <b-button block variant="danger" @click.prevent="canceled">Hủy</b-button>
                </b-col>
            </b-row>
        </b-form-group>
    </b-form>
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
              <b-form-select v-model="perPage3" id="perPageSelect" size="sm" :options="pageOptions"></b-form-select>
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
                  <b-button :disabled="!filter" @click="filter = ''">Tìm Kiếm</b-button>
                </b-input-group-append>
              </b-input-group>
            </b-form-group>
          </b-col>
        
        </b-row>
    <div v-if="statusOk">
        <b-table show-empty small stacked="md" :items="hisIns" :fields="fields" :current-page="currentPage3" :per-page="perPage3" :filter="filter" :filterIncludedFields="filterOn" :sort-by.sync="sortBy" :sort-desc.sync="sortDesc" :sort-direction="sortDirection" @filtered="onFiltered">
            <template v-slot:cell(name)="row">{{ row.value.first }} {{ row.value.last }}</template>
            <template v-slot:cell(name)="row">{{ row.value}}</template>

            <template v-slot:cell(actions)="row">
                <b-button size="sm" @click="info(row.item, row.index, $event.target)" class="mr-1">
                    <b-icon icon="document-text"></b-icon>
                </b-button>
            </template>
        </b-table>
        <b-row>
          <b-col sm="6" md="6" class="my-1">
            <div></div>
          </b-col>
          <b-col sm="6" md="6" class="my-1">
            <b-pagination
              v-model="currentPage3"
              :total-rows="totalRows3"
              :per-page="perPage3"
              class="mt-4"
              align="right"
            ></b-pagination>
          </b-col>
        </b-row>

    </div>
    <b-modal ref="respone" title="Thông báo">
        <b-row>
            <b-col>
                <label>{{responeMessage}}</label>
            </b-col>
        </b-row>
        <template v-slot:modal-footer>
            <div hidden></div>
        </template>
    </b-modal>
</b-card>
</template>

<script>
import datetime from 'vuejs-datetimepicker';


import apiHelper from '../../helper/call_api'

export default {
    name: "History",
    components: {
        datetime
    },
    data() {
        return {
            toDate: (new Date()).toISOString().slice(0, 10),
            fromDate: (new Date()).toISOString().slice(0, 10),
            statusOk: false,
            send: "",
            responeMessage: "",
            payInfo: {
                UserName: "",
                AccountNumber: "",
                Money: 0
            },
            
            hisIns: [],
            hisOuts: [],
            hisDeptIns: [],
            hisDeptOuts: [],
            totalRows3: 1,
            currentPage3: 1,
            perPage3: 6,
            pageOptions: [1, 2, 5, 10, 15, 50, 70],
            sortBy: "",
            sortDesc: false,
            sortDirection: "asc",
            filter: null,
            filterOn: [],
            fields: [{
                    key: "SourceAccountNumber",
                    label: "STT",
                    sortable: true,
                    sortDirection: "desc"
                },
                {
                    key: "SourceAccountName",
                    label: "Tên ",
                    sortable: true,
                    sortDirection: "desc"
                },
                {
                    key: "SourceBankName",
                    label: "Bank Liên Kết ",
                    sortable: true,
                    sortDirection: "desc"
                },
                 {
                    key: "Money",
                    label: "Tiền",
                    sortable: true,
                    sortDirection: "desc"
                },
                {
                    key: "ConfirmTime",
                    label: "Time",
                    sortable: true,
                    sortDirection: "desc"
                },
                  {
                    key: "Description",
                    label: "Mô tả",
                    sortable: true,
                    sortDirection: "desc"
                },
             
            ],
        };
    },

    methods: {
        onSubmit(evt) {
            
            var _this = this;
            var fromDate = this.fromDate;
            var toDate = this.toDate;
            evt.preventDefault();
            this.$validator.validateAll().then(result => {
                if (!result) {
                    return;
                }

                // if (this.user.AccountNumber == "" && this.user.Username == "") {
                ///   return;
                // }

                // if (this.type == 1) this.user.UserName = this.valueType;
                // else this.user.AccountNumber = this.valueType;

                // get histories in
                apiHelper
                    .call_api(`Administrators/CrossCheckings/In?from=${fromDate}&to=${toDate}`, "get", '')
                    .then(res => {
                        this.hisIns = res.data;
                        this.statusOk = true;
                    })
                    .catch(err => {
                        console.log(err);
                    });
            })
        },
        canceled() {
            // Reset our form values
            this.user.AccountNumber = "";
            this.user.Username = "";
            this.statusOk = false;
            this.send = this.valueType;
            this.payInfo = {};
            this.valueType = "";
            this.type = 1;
            this.$nextTick(() => {
                this.$validator.reset();
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
        onFiltered(filteredItems) {
            // Trigger pagination to update the number of buttons/pages due to filtering
            this.totalRows3 = filteredItems.length;
            this.currentPage3 = 1;
        }
    }
};
</script>

<style scoped>
.bcard-shadow {
    margin-top: 15px;
    box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19);
}
</style>
