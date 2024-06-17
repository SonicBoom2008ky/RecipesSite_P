import { addButtonIng, cupSize, NewIngridientName, divListIng } from "./addrecipe";

addButtonIng.onclick = function () {

    let Ingridient = cupSize.value + NewIngridientName.value;
    divListIng.appendChild(Ingridient);

    cupSize.value = '';
    NewIngridientName.value = '';
};
