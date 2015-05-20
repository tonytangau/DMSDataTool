var DmsForm = React.createClass({
	getInitialState: function() {
		return {
			api: 'https://localhost:44302/api/dms',//'hub3.azurewebsites.net/api/dms', 
			tenantId: '98D11A03-5D71-4D58-8A1C-668103413360',
			isDisabled: false
		};
	},
	onApiChange: function(e) {
		this.setState({api: e.target.value});

		if(e.target.value.length == 0 || this.state.tenantId.length == 0) {
			this.state.isDisabled = true;
		}
		else {
			this.state.isDisabled = false;
		}
	},
	onTenantIdChange: function(e) {
		this.setState({tenantId: e.target.value});

		if(e.target.value.length == 0 || this.state.api.length == 0) {
			this.state.isDisabled = true;
		}
		else {
			this.state.isDisabled = false;
		}
	},
	handleCancel: function(e) {
		e.preventDefault();

		$.ajax({
			  url: "api/dms/cancel",
			  dataType: 'json',
			  type: 'POST',
			  data: { tenantId: this.state.tenantId, url: this.state.api },
			  success: function(data) {
			  }.bind(this),
			  error: function(xhr, status, err) {
			  }.bind(this)
			});
	  },
	handleSubmit: function(e) {
		e.preventDefault();

		$.ajax({
			  url: "api/dms",
			  dataType: 'json',
			  type: 'POST',
			  data: { tenantId: this.state.tenantId, url: this.state.api },
			  success: function(data) {
				
				// TODO: Add to display
				//this.setState({data: data});
			  }.bind(this),
			  error: function(xhr, status, err) {
			  }.bind(this)
			});
	  },
	render: function(){
		return(
			<form>
				<div className="row">
					<div className="six columns">
						<label>DMS API</label>
						<input id="api-input" className="u-full-width" type="text" onChange={this.onApiChange} placeholder="DMS API Url" value={this.state.api} />
					</div>
					<div className="six columns">
						<label>TenantId</label>
						<input id="tenantId-input" className="u-full-width" type="text" onChange={this.onTenantIdChange} placeholder="Tenant Id" value={this.state.tenantId} />
					</div>
				</div>
				<input type="submit" onClick={this.handleSubmit} disabled={this.state.isDisabled} className="button-primary" value="Start"></input>&nbsp;&nbsp;
				<input type="button" onClick={this.handleCancel} value="Stop"></input>
			</form>
		);
	}
});

React.render(
	<DmsForm />,
	document.getElementById('container')
);