$.ajax.fake.registerWebservice('http://myselfie.com/event/1/wall/1', function(data) {
	return {
		packets: 
			[{
				slides: [
			    	'assets/images/slides/02.jpg',
			    	'assets/images/slides/04.jpg',
			    	'assets/images/slides/06.jpg',
			    	'assets/images/slides/08.jpg',			    				    				    	
			    	'assets/images/slides/10.jpg',
			    	'assets/images/slides/01.jpg',
			    	'assets/images/slides/03.jpg',
			    	'assets/images/slides/05.jpg',
			    	'assets/images/slides/07.jpg'			    	
			    ],
			    sponsor: 'assets/images/ads/advertisement1.jpg'
			}]
		};
});