import React, { Component } from 'react';
import shortid from 'shortid';

export class Select extends Component {
  constructor(props) {
    super(props);
    this.handleChange = this.handleChange.bind(this);
  }

  handleChange(e) {
    this.props.onChange(e);
  }

  render() {
    let values;
    if (this.props.options) {
      values = this.props.options.map(option => (
        <option key={shortid()} value={option.teamID}>{option.name}</option>
      ));
    }
    return (
      <div>
        <select
          id={this.props.id}
          className="form-control custom-select select"
          value={this.props.value}
          onChange={this.props.onChange}
          required={!this.props.notRequired}
        >
          <option value="" defaultValue>{this.props.defaultValue}</option>
          {values}
        </select>
        <div className="invalid-feedback">
          {this.props.error}
        </div>
      </div>
    );
  }
}

export default Select;