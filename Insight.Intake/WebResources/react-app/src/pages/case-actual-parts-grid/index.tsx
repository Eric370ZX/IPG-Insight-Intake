import React from 'react';
import ReactDOM from 'react-dom';

import ActualPartsGridScreen from '../../components/actual-parts-grid-screen/actual-parts-grid-screen';

import * as serviceWorker from '../../serviceWorker';

ReactDOM.render(
  <React.StrictMode>
    <ActualPartsGridScreen/>
  </React.StrictMode>,
  document.getElementById('root')
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();