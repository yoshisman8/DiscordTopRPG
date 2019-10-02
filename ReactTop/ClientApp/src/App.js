import React, { Component } from 'react';
import { Route } from 'react-router';
import Navigation from './components/navbar';

export default class App extends Component {
	static displayName = App.name;

	constructor() {
		super();
		this.handleIncrement = this.handleIncrement.bind(this);
	}
	state = {
		counter: 0,
		tags: ['tag1', 'tag2', 'tag3']
	}
	handleIncrement = () => {
		this.setState({ counter: this.state.counter + 1 });
	}
	formatcount() {
		const { counter } = this.state;
		return counter === 0 ? "Zero" : counter;
	}
	render() {
		return (
			<React.Fragment>
				<Navigation />
				<h1> Hello World</h1>
				<span className={this.getBadgeClasses()}>{this.formatcount()}</span>
				<button className="btn btn-secondary btn-sm" onClick={this.handleIncrement}>Increase</button>
			</React.Fragment>
		);
	}

	getBadgeClasses() {
		let classes = "badge m-2 badge-";
		classes += (this.state.counter === 0) ? "warning" : "primary";
		return classes;
	}
}
