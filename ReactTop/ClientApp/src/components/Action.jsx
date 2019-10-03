import React, { Component } from 'react';
import { Container, Row, ButtonGroup, Form, FormGroup, Label, Input, Button } from 'reactstrap';

export default class Action extends Component {
	constructor(props) {
		super(props);
		this.state = {
			id: props.id
		};
	}
	render() {
		return (
			<div>
				<Form>
					<FormGroup>
						<Label for="name">Action Name </Label>
						<Input id="name" name="name" type="text" />
					</FormGroup>
					<FormGroup>
						<Label for="description">Description</Label>
						<textarea style="overflow:auto;resize:none" rows="6" id="description" name="description" className="form-control"> </textarea>
					</FormGroup>
					<FormGroup>
						<Label for="economy">Action Economy</Label>
						<Input type="select" name="economy" id="economy" multiple>
							<option> Simple Action </option>
							<option> Complex Action </option>
							<option> Reaction </option>
							<option> Free Action </option>
						</Input>
					</FormGroup>
					<ButtonGroup>
						<Button onClick={this.props.onAdd(this.props.id)} color="success"> Add </Button>
					</ButtonGroup>
				</Form>
			</div>
		);
	}
}