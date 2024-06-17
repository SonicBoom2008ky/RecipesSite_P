import { send } from "./_utils";

/**@type {HTMLInputElement} */
let titleInput = document.getElementById("RecipeNameInput");

/**@type {HTMLInputElement} */
let imageSource = document.getElementById("ImageSorce");

/**@type {HTMLDivElement} */
let divListIng = document.getElementById("divListIng");

/**@type {HTMLInputElement} */
let NewIngridientName = document.getElementById("IngridientName");

/**@type {HTMLOptionElement} */
let cupSize = document.getElementById("cupSize");

/**@type {HTMLTextAreaElement} */
let methodTextarea = document.getElementById("methodTextarea");

/**@type {HTMLSelectElement} */
let difficulty = document.getElementById("difficultySelection");

/**@type {HTMLInputElement} */
let timeInput = document.getElementById("timeInput");

/**@type {HTMLButtonElement} */
let NewRecipeButton = document.getElementById("NewRecipeButton");

/**@type {HTMLButtonElement} */
let addButtonIng = document.getElementById("addButtonIng");

addButtonIng.onclick = function () {

  let Ingridient = cupSize.value + NewIngridientName.value;
  divListIng.appendChild(Ingridient);

  cupSize.innerText = '';
  NewIngridientName.innerText = '';
}

NewRecipeButton.onclick = function () {

  let recipe = {
    title: titleInput.value,
    imageSource: imageSource.value,
    Ingridients: divListIng.value,
    method: methodTextarea.value,
    difficulty: difficulty.value,
    time: timeInput.value
  };

  send("/addrecipe", recipe);

  top.location.href = "index.html";
}
