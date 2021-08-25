import Vue from 'vue';
import { AxiosError, AxiosResponse } from 'axios';
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

  public $_helper_toastInfo(response: AxiosResponse): void {
    const message = response.data.message ?? '';
    this.$bvToast.toast(message, {
      title: this.$t('INFO_MESSAGE_TITLE').toString(),
      variant: 'success',
      solid: true
    });
  }
}
