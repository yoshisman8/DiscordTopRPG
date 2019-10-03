import React, { Component } from 'react';
import Action from './Action';
import { Container, Badge, Collapse, Form, FormGroup, Label, Input, Button, Alert } from 'reactstrap';

export default class Skill extends Component {
	constructor(props) {
		super(props);
		this.state = {
			actions: [],
			collapse: false
		};
	}
	badgeEcon(econ) {
		if (econ === "Simple Action") return <Badge color="primary">S</Badge>
		else if (econ === "Complex Action") return <Badge color="secondary">C</Badge>
		else if (econ === "Reaction") return <Badge color="success">R</Badge>
		else if (econ === "Free Action") return <Badge color="info">F</Badge>
	}
	handleAdd = (action) => {
		console.log("Added action", action);
		this.setState(st => {
			const actions = st.actions.concat({ id: st.actions.length, name: action.name, description: action.description, economy: action.economy });
			return {
				actions,
				collapse: false
			};
		});

	}
	deleteAction = (actionId) => {
		console.log("Deleted action id " + actionId);
		this.setState(state => {
			const actions = state.actions.filter((item, j) => actionId !== j);
			return {
				actions,
			};
		});
	}
	toggle = () => {
		this.setState(state => ({ collapse: !state.collapse }));
	}
	render() {
		return (
			<Container className="rounded border bg-dark text-light">
				<Form>
					<FormGroup>
						<Label for="name">Skill Name</Label>
						<Input type="text" id="name" name="name" />
					</FormGroup>
					<FormGroup>
						<Label for="flavor">Skill Description</Label>
						<textarea rows="6" id="flavor" name="flavor" className="form-control"> </textarea>
					</FormGroup>
					<FormGroup>
						<Label for="score">Ability Score</Label>
						<Input type="select" name="score" id="score">
							<option> Strength </option>
							<option> Dexterity </option>
							<option> Agility </option>
							<option> Constitution </option>
							<option> Memory </option>
							<option> Intuition </option>
							<option> Charisma </option>
						</Input>
					</FormGroup>

					<FormGroup>
						<Label> Actions </Label>
						<Button className="btn-sm ml-2" onClick={this.toggle}>
							Add
							</Button>
					</FormGroup>

					<ul>
						{this.state.actions.map(action => (
							<li key={action.id}>
								<Alert color="light">
									<span className="text-dark">
										{this.badgeEcon(action.economy)} {action.name}
									</span>
									<Button size="sm" className="btn-sm ml-2" color="danger" onClick={() => this.deleteAction(action.id)} >
										Delete
									</Button>
								</Alert>
							</li>
						))}
					</ul>
				</Form>
				<Collapse isOpen={this.state.collapse}>
					<Action onAdd={this.handleAdd} />
				</Collapse>
				<Button> Create </Button>
			</Container>
		);
	}
}
