import Cookies from "./_cookies";
import { getQuery, send } from "./_utils";

/**
 * @typedef Book
 * @property {string} title
 * @property {string} imageSource
 * @property {string} Ingridients
 * @property {string} method
 * @property {string} difficulty
 * @property {string} time
 * @property {boolean} isFavorite
 */

/**@type {HTMLTitleElement} */
let title = document.getElementsByTagName("title");

/**@type {HTMLHeadingElement} */
let titleH1 = document.getElementById("titleH1");

/**@type {HTMLImageElement} */
let coverImg = document.getElementById("coverImg");

/**@type {HTMLButtonElement} */
let favoriteButton = document.getElementById("favoriteButton");

/**@type {HTMLButtonElement} */
let unfavoriteButton = document.getElementById("unfavoriteButton");

/**@type {HTMLDivElement} */
let descriptionDiv = document.getElementById("descriptionDiv");

let userId = Cookies.get("id");

/**@type {{recipeId: string}} */
let query = getQuery();

let recipeId = Number(query.recipeId);


favoriteButton.onclick = async function () {
  await send("/addToFavorites", {
    userId: userId,
    recipeId: recipeId
  });

  favoriteButton.disabled = true;
  unfavoriteButton.disabled = false;
}

unfavoriteButton.onclick = async function () {
  await send("/removeFromFavorites", {
    userId: userId,
    recipeId: recipeId
  });

  favoriteButton.disabled = false;
  unfavoriteButton.disabled = true;
}

async function appendBook() {
  /**@type {Book} */
  let book = await send("/getrecipe", recipeId);

  document.title = book.title;
  titleH1.innerText = book.title;
  coverImg.src = book.imageSource;
  descriptionDiv.innerText = book.description;

  if (userId == undefined) {
    favoriteButton.style.display = "none";
    unfavoriteButton.style.display = "none";
    return;
  }

  /**@type {boolean} */
  let isFavorite = await send("/getIsFavorite", {
    userId: userId,
    recipeId: recipeId,
  });

  favoriteButton.disabled = isFavorite;
  unfavoriteButton.disabled = !isFavorite;
}

appendBook();