import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import shortid from 'shortid';
import _ from 'lodash';
import { DebounceInput } from 'react-debounce-input';
import { UserCard } from '../components/Profile/UserCard';
import * as actionCreators from '../actions/userPool';

const mapStateToProps = state => ({
  allUsers: state.userPoolContainerReducer.allUsers,
  filteredUsers: state.userPoolContainerReducer.filteredUsers,
  filterString: state.userPoolContainerReducer.filterString,
  userPoolLoader: state.userPoolContainerReducer.userPoolLoader
});

const mapDispatchToProps = dispatch => bindActionCreators(actionCreators, dispatch);

export class UserPoolContainer extends Component {
  constructor(props) {
    super(props);
    this.handleFilterUsers = this.handleFilterUsers.bind(this);
  }

  async componentDidMount() {
    const { loadUserPool } = this.props;
    await loadUserPool();
  }

  handleFilterUsers(e) {
    const { filterUsers } = this.props;
    filterUsers(e.target.value.toLowerCase());
  }

  render() {
    const { filteredUsers, userPoolLoader } = this.props;
    if (userPoolLoader) {
      return <div className="m-5"><div className="Loader" /></div>;
    }

    const userCards = _.map(
      filteredUsers,
      user => (
        <UserCard
          key={shortid()}
          user={user}
          color={user.color}
        />
      )
    );
    return (
      <div className="container bg-light">
        <div className="search m-3 mb-4">
          <span className="fa fa-search" />
          <DebounceInput
            className="UserPool__Search form-control"
            type="search"
            name="search"
            placeholder="Search..."
            debounceTimeout={600}
            onChange={this.handleFilterUsers}
          />
        </div>
        <div className="center col">
          <div className="row">
            {userCards}
          </div>
        </div>
      </div>
    );
  }
}

UserPoolContainer.propTypes = {
  loadUserPool: PropTypes.func.isRequired,
  filterUsers: PropTypes.func.isRequired,
  filteredUsers: PropTypes.arrayOf(
    PropTypes.shape({
      id: PropTypes.string.isRequired
    })
  ).isRequired,
  userPoolLoader: PropTypes.bool.isRequired
};

export default connect(mapStateToProps, mapDispatchToProps)(UserPoolContainer);
