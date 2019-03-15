import React, { Component } from 'react';
import PropTypes from 'prop-types';
import shortid from 'shortid';
import _ from 'lodash';
import { UserCard } from '../UserCard';

export default class FriendList extends Component {
  constructor(props) {
    super(props);
    this.state = {
      friends: ''
    };
  }

  async componentDidMount() {
    const { user } = this.props;
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/user/friends/${user.id}`)
      .then(res => res.json())
      .then(res => this.setState({
        friends: res
      }));
  }

  render() {
    const { friends } = this.state;
    const allFriends = _.map(friends,
      friend => (
        <UserCard
          key={shortid()}
          user={friend}
        />
      ));
    return (
      <div className="row">
        {allFriends.length > 0 ? allFriends : "User doesn't have any friends!"}
      </div>
    );
  }
}

FriendList.propTypes = {
  user: PropTypes.shape({
    id: PropTypes.string.isRequired
  }).isRequired
};
