
# MVP

## FrontEnd

### 4 views
* Ingredients list
* Recipies list
* Recipy view
* Meal planer

#### Ingredients
``[{ingredient_table_row}, ..., {ingredient_table_row}]`` in state.

#### Recipies List
``[{recipy_table_row}, ..., {recipy_table_row}]`` in state.

#### Indivudual Recipy view
Query ``recipy_to_ingredient`` table, modify amount

#### Meal planer
Like a cart mechanic. Persist in local storage for now.

## Backend
* Get Recipies, Ingredients (all and by Id)
* Post Ingredients and recipies