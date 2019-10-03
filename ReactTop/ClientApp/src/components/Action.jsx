import React, { Component } from 'react';
import { Container, Row, ButtonGroup, Form, FormGroup, Label, Input, Button } from 'reactstrap';

export default class Action extends Component {
	constructor(props) {
		super(props);
		this.state = {
			id: props.id,
			name: "",
			description: "",
			economy: "Simple Action"
		};

		this.handleInputChange = this.handleInputChange.bind(this);
	}

	handleInputChange(event) {
		const target = event.target;
		const value = target.type === 'checkbox' ? target.checked : target.value;
		const name = target.name;

		this.setState({
			[name]: value
		});
		console.log("Changed field " + name + " to value '" + value + "'.")
	}
	render() {
		return (
			<div>
				<Form>
					<FormGroup>
						<Label for="name">Action Name </Label>
						<Input id="name" name="name" type="text" onChange={this.handleInputChange} />
					</FormGroup>
					<FormGroup>
						<Label for="description">Description</Label>
						<textarea rows="6" id="description" name="description" className="form-control" onChange={this.handleInputChange}> </textarea>
					</FormGroup>
					<FormGroup>
						<Label for="economy">Action Economy</Label>
						<Input type="select" name="economy" id="economy" onChange={this.handleInputChange}>
							<option> Simple Action </option>
							<option> Complex Action </option>
							<option> Reaction </option>
							<option> Free Action </option>
						</Input>
					</FormGroup>
					<ButtonGroup>
						<Button onClick={() => this.props.onAdd({ name: this.state.name, description: this.state.description, economy: this.state.economy })} color="success"> Add </Button>
					</ButtonGroup>
				</Form>
			</div>
		);
	}
}
