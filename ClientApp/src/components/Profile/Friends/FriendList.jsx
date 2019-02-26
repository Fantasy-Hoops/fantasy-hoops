import React, { Component } from 'react';
import { UserCard } from './../UserCard';
import shortid from 'shortid';
import _ from 'lodash';

export class FriendList extends Component {
  constructor(props) {
    super(props);
    this.state = {
      friends: ''
    }
  }

  async componentWillMount(prevProps) {
    await fetch(`${process.env.REACT_APP_SERVER_NAME}/api/user/friends/${this.props.user.id}`)
      .then(res => {
        return res.json()
      })
      .then(res => {
        this.setState({
          friends: res
        })
      });
  }

  render() {
    let friends = _.map(this.state.friends,
      (friend) => {
        return <UserCard
          key={shortid()}
          user={friend}
        />
      }
    );
    return (
      <div className="row">
        <div className="row">
          {friends.length > 0 ? friends : "User doesn't have any friends!"}
        </div>
      </div>
    );
  }
}