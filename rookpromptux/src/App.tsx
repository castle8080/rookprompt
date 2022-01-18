import React from 'react';
import logo from './logo.svg';
import './App.css';
import FirstNavigation from './FirstNavigation';

class App extends React.Component {
    render() {
      return (
        <div className="App">
          <h1>Writing Prompt</h1>
          <FirstNavigation name="Bob"/>
        </div>
      );
    }
}

export default App;
