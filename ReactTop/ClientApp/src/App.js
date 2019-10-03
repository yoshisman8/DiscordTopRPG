import React, { Component } from 'react';
import { Route } from 'react-router';
import Navigation from './components/navbar';
import Skill from './components/Skill';

export default class App extends Component {
	static displayName = App.name;

	constructor() {
		super();
	}
	state = {

	}

	render() {
		return (
			<React.Fragment>
				<Navigation />
				<Skill onAdd={this.handleAdd}/>
			</React.Fragment>
		);
	}

}
