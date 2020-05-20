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
import HistoryDeptIn from "../../components/Employee/HistoryDeptIn.vue";
import HistoryIn from "../../components/Employee/HistoryIn.vue";

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
                    label: "SourceAccountNumber",
                    sortable: true,
                    sortDirection: "desc"
                },
                {
                    key: "SourceAccountName",
                    label: "SourceAccountName",
                    sortable: true,
                    sortDirection: "desc"
                },
                {
                    key: "ConfirmTime",
                    label: "ConfirmTime",
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
