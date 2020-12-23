<template>
  <b-form ref="form" @submit="exitEditMode" novalidate>
    <b-form-group :label="label" :description="description" label-for="inputField">
      <b-input-group>
        <b-icon-pencil-fill v-if="!editable" @click="clickPen" :title="$t('EDIT')" />
        <b-form-textarea
          ref="the_input"
          :plaintext="!editable"
          :placeholder="placeholder"
          @click="enterEditMode"
          @blur="exitEditMode"
          @keyup.esc="exitEditMode"
          class="editable"
          id="inputField"
          v-bind:value="value"
          @input="val => { this.$emit('input', val) }"
          type="text"
          name="inputField"
          :rows="rows"
          :required="required"
          :minlength="minlength"
          :maxlength="maxlength"
        ></b-form-textarea>
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
    rows: Number,
    feedback: String,
    description: String,
    placeholder: String,
    minlength: { type: Number, default: 0 },
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
  display: inline-block;
  position: absolute;
  right: 15px;
  top: 15px;
  z-index: 10;
  color: lightgrey;
}

textarea {
  border: 1px solid transparent;
}

.form-control-plaintext {
  padding-left: 0.8em;
}

textarea:hover {
  border: 1px solid lightgrey;
}

.input-group:hover .bi-pencil-fill {
  color: black;
}
</style>
