<template>
  <b-form inline ref="form" @submit="exitEditMode" novalidate>
    <b-form-group :description="description" label-for="inputField">
      <b-input-group :prepend="label" size="sm">
        <b-form-input
          ref="the_input"
          :plaintext="!editable"
          @click="enterEditMode"
          @blur="exitEditMode"
          @keyup.esc="exitEditMode"
          id="inputField"
          v-bind:value="value"
          @input="val => { this.$emit('input', val) }"
          type="text"
          name="inputField"
          :required="required"
          size="sm"
          :minlength="minlength"
          :maxlength="maxlength"
        ></b-form-input><b-icon-pencil-fill v-if="!editable" @click="clickPen" :title="$t('EDIT')" />
        <b-form-invalid-feedback>{{ feedback }}</b-form-invalid-feedback>
      </b-input-group>
    </b-form-group>
  </b-form>
</template>

<script lang="ts">
import Vue from 'vue';
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
    clickPen() {
      this.editable = true;
    },
    enterEditMode() {
      if (!this.editable) {
        this.initialFieldValue = this.value;
        this.editable = true;
      }
    },
    exitEditMode(evt: Event) {
      if (!this.$quizrhelpers.formIsValid(evt, this.$refs.form as HTMLFormElement)) {
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
.bi-pencil-fill {
  cursor: pointer;
  display: none;
  position: absolute;
  right: 7px;
  top: 7px;
  z-index: 10;
  color: gray;
}

input {
  border: 1px solid lightgrey;
  /* border-color: lightgrey; */
}

.form-control-plaintext {
  padding-left: 0.8em;
}

input:hover {
  border: 1px solid grey;
}
.input-group:hover .bi-pencil-fill {
  display: inline-block;
  color: black;
}
</style>
