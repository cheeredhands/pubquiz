import Vue from 'vue';
import { AxiosError } from 'axios';
import { ApiResponse } from '../models/apiResponses';
import Component from 'vue-class-component';
/* eslint camelcase: "off" */
@Component
export default class HelperMixin extends Vue {
  public $_helper_toastError(error: AxiosError<ApiResponse>): void {
    const errorCode = error.response?.data.code ?? 'UNKNOWN_ERROR';
    const errorMessage = error.response?.data.message ?? '';
    this.$bvToast.toast(errorMessage, {
      title: `${this.$t('ERROR_MESSAGE_TITLE').toString()}: ${this.$t(errorCode).toString()}`,
      variant: 'danger',
      solid: true
    });
  }
}
