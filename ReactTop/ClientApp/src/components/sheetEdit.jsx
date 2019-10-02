import React, { Component } from 'react';
import { Container, Row, Col, Form, FormGroup, Label, Input, FormText } from 'reactstrap';

export default class SheetEdit extends Component {
	constructor(props) {
		super(props);
		this.state = {
		};
	}
	render() {
		return (
			<Container>
				<Row>
					<Col sm="9">
						<FormGroup>
							<Label for="name"> Name </Label>
							<Input type="text" id="name" />
						</FormGroup>
					</Col>
					<Col sm="3"></Col>
				</Row>
			</Container>
		);
	}
}
