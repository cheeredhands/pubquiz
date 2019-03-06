import Vue, { PluginObject } from 'vue';

export class QuizrHelpers {

    public formIsValid(evt: Event) : boolean {
        // check validation
        evt.preventDefault();
        evt.stopPropagation();

        const form = evt.srcElement as HTMLFormElement;

        if (form.checkValidity() === false) {
            // https://getbootstrap.com/docs/4.3/components/forms/#custom-styles
            form.classList.add("was-validated");
            console.log("invalid form, canceling.");
            return false;
        }
        console.log("valid form.");
        return true;
    }
}

const quizrHelpersInstance = new QuizrHelpers();

const Plugin: PluginObject<any> = {
    install: vue => {
const quizrHelpersInstance = new QuizrHelpers();
      vue.$quizrhelpers = quizrHelpersInstance;
    }
  };
  Plugin.install = vue => {
    vue.$quizrhelpers = quizrHelpersInstance;
    Object.defineProperties(vue.prototype, {
      $quizrhelpers: {
        get() {
          return quizrHelpersInstance;
        }
      }
    });
  };
  
  Vue.use(Plugin);
  
  export default Plugin;