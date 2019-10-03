import React, { Component } from 'react';
import Action from './Action';
import { Container, Row, Col, Form, FormGroup, Label, Input, Button } from 'reactstrap';

export default class Skill extends Component {
	constructor(props) {
		super(props);
		this.state = {
			actions: []
		};
	}
	render() {
		return (
			<Container className="rounded border bg-dark">
				<form>
					<FormGroup>
						<Label for="name">Skill Name</Label>
						<Input type="text" id="name" name="name"/>
					</FormGroup>
					<FormGroup>
						<Label for="flavor">Skill Description</Label>
						<textarea style="overflow:auto;resize:none" rows="6" id="flavor" name="flavor" className="form-control"> </textarea>
					</FormGroup>
					<FormGroup>
						<Label for="score">Ability Score</Label>
						<Input type="select" name="score" id="score" multiple>
							<option> Strength </option>
							<option> Dexterity </option>
							<option> Agility </option>
							<option> Constitution </option>
							<option> Memory </option>
							<option> Intuition </option>
							<option> Charisma </option>
						</Input>

						<ul>
							{this.state.actions.map(action => (
								<li> {action.name} {badgeEcon(action.economy)} <Button close/> </li>
							))}
						</ul>

						<Action onAdd={this.handleAdd}/>
					</FormGroup>
					<Button> Create </Button>
				</form>
			</Container>
		);
	}
}
