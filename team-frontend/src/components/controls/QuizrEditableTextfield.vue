<template>
  <b-form ref="form" @submit="exitEditMode" novalidate>
    <b-form-group :label="label" :description="description" label-for="inputField">
      <b-input-group>
        <font-awesome-icon v-if="!editable" icon="pen" @click="enterEditMode" :title="$t('EDIT')" />
        <b-form-input ref="the_input"
          :plaintext="!editable"
          @click="enterEditMode"
          @blur="exitEditMode"
          id="inputField"
          v-bind:value="value"
          @input="val => { this.$emit('input', val) }"
          type="text"
          name="inputField"
          :required="required"
          size="lg"
          :minlength="minlength"
          :maxlength="maxlength"
        ></b-form-input>
        <b-form-invalid-feedback>{{ feedback }}</b-form-invalid-feedback>
      </b-input-group>
    </b-form-group>
  </b-form>
</template>

 <script lang="ts">
import Vue from 'vue';
import { resolve } from 'dns';
export default Vue.extend({
  data() {
    return {
      initialFieldValue: '',
      editable: false
    };
  },
  props: {
    value: String,
    label: String,
    description: String,
    feedback: String,
    minlength: Number,
    maxlength: Number,
    required: Boolean
  },
  created() {
    this.initialFieldValue = this.value;
  },
  methods: {
    enterEditMode() {
      if (!this.editable) {
        (this.$refs.the_input as any).focus();
        this.initialFieldValue = this.value;
        this.editable = true;
      }
    },
    exitEditMode(evt: Event) {
      if (!this.$quizrhelpers.formIsValid(evt, this.$refs.form as any)) {
        return;
      }
      if (this.initialFieldValue === this.value) {
        this.editable = false;
        return;
      }
      this.$emit('apply');
      this.editable = false;
    }
  }
});
</script>

<style scoped>
.fa-pen {
  cursor: pointer;
  display: inline-block;
  position: absolute;
  right: 15px;
  top: 15px;
  z-index: 10;
  color: lightgrey;
}

input {
  border: 1px solid transparent;
}

.form-control-plaintext {
    padding-left: 0.8em;
}

input:hover {
  border: 1px solid lightgrey;
}
.input-group:hover .fa-pen {
  /* display: inline-block; */
  color: black;
}
</style>