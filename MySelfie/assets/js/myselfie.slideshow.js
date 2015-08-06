// Developer: Jason Newell
// FYI: http://stackoverflow.com/questions/1873983/what-does-the-leading-semicolon-in-javascript-libraries-do
;(function($, window, document, undefined ) {
	$.widget('myselfie.slideshow', {
		// Defaults
		options: {
		    interval: 5000,                 // length that each slide stays 'full screen' after first appearing (-> transitionDuration -> gridShowDuration -> gridStagger)
		    transitionDuration: 1000,       // time it takes to 'zoom in' after the slide is initially shown			             
			gridShowDuration: 2000,         // time between finishing 'zoom in' and 'new slide' appearing
			gridStagger: 150,               // used for the animation after all slides are shown when they each fade out diagonally from top left to bottom right
			finalInterval: 3000,            // length that the ad is shown after 'grid stagger' before sliding away and next packet begins
			animationDuration: 800,         // length of fade in / fade out - this is independent of movement durations 
			maxStalePackets: 5,            // number of packets that can exist in memory - this is applied after each new set of packets arrives
			caption: '',                    // the "hashtag", shown at the top of the image grid
			title: '',        // the "title", shown in big letters next to the logo near the top of the screen
			left: '',                       // the "call to action", shown along the left of the image grid and rotated 90 degrees
            right: '',                      // not currently implemented, supposed to show along the right of the image grid and rotated 90 degrees
			topColor: '000999',             // the "top color", this is the color above and to the left of the image grid - the caption, title, left and logo are inside this area
            bottomColor: '888fff',          // the "bottom color", this is the color behind the image grid - in the bottom right
            wallID: 0,                      // this determines which wall to pull images from
            isVideoBakcground: false,       // is the background a video? if not it is an image based on the wallID
            packets: [],                    // this is the original set of images when this plugin is initially loaded
            hasStarted: false               // once it has recieved a packet, this is set to true... until then, looks for new packets every second
		},
		
		_create: function () {
			this.intervalID		 = 
/* 			this.packetTimeoutID =  */
			this.currentPacket   = 
			this.nextPacket		 = null;			
			this.slideIndex	     = null;
			this.slideAnchorPoint = function(slide) { // There's probably a better place to put this.
				var index = $(slide).index(); // Index inside parent grid
				var row = Math.floor(index / 3);
				var col = index % 3;
				return parseInt(col * 50) + '% ' + parseInt(row * 50) + '%';			
			};
			
			var instance = this;
			
			var packetContainer = this.element.find('#imagePackets');
		    //$('<img>').attr('src', 'assets/images/logo-zero-quality.png').insertBefore(packetContainer);
			//$('<img>').attr('src', '/Wall/Image/' + this.options.wallID).insertBefore(packetContainer);

			$('<h1>').html(this.options.title).addClass('title').insertBefore(packetContainer);
			$('<h2>').html(this.options.caption).addClass('hashtag').insertBefore(packetContainer);
			$('<h3>').html(this.options.left).addClass('left').insertBefore(packetContainer);
			$('<h4>').html(this.options.right).addClass('right').insertBefore(packetContainer);
			//$('<h3>').html('Use Hashtag to Upload Your Photo <span>NOW!</span>').addClass('left').insertBefore(packetContainer);

			$(window).resize(function() {
				instance._sizeFrame();
			});

			for (var i = 0; i < this.options.packets.length; i++) {
			    this._buildPacket(this.options.packets[i].Data[0], this.options.packets[i].PacketId, this.options.packets[i].Status);
			}
			
			this.element.children().wrapAll($('<div>').addClass('frame'));

			if (this.options.topColor != '') {
			    $(".frame").css('background-color', "#" + this.options.topColor);
			}
			if (this.options.bottomColor != '') {
			    $("#imagePackets").css('background-color', "#" + this.options.bottomColor);
			}

			this._sizeFrame();			

			if (this.options.packets.length > 0) {
			    //console.log("Startup with packets");
			    this.options.hasStarted = true;
			    this._start();
			}
			else {
			    //console.log("Startup with no packets");
			}
            /*
			else {
			    this._requestRefresh();
			}
            */
            //moving to auto _requestRefresh
            setInterval($.proxy(this._requestRefresh, this), 5000);
		},
				
		_setupPacket: function(packet) {		
			packet
	 			.children('span')
	 			.each(function() {
	 				$(this).css('background-image', 'url(' + $(this).children().attr('src') + ')');	
	 			});
	 				 
			packet
				.find('span:not(.sponsor)')
				.clone()
				.insertBefore($(packet).children('.sponsor'))
					.chunkedWrap(3, '<div class="grid-row"></div>') // Divide standard slides into 4 chunks for our 4x4 grid
	 				.wrapAll('<div class="grid"></div>');
	 		
	 		packet.children('.grid').css('background-image', "url('" + packet.find('.sponsor > img').attr('src') + "')");		
	 		packet.children('.sponsor').remove();	 	 							 			 				
	 			 				
	 		this._flipZ(packet.children());
		},

		_buildPacket: function (data, packetId, status) {
		    var packet = $('<div>').addClass('packet')
                .attr("data-packetId", packetId)
                .attr("data-status", status);

		    for (var j in data.slides) {
		        packet.append(
                    $('<span>').addClass('imageContainer').append(
                        $('<img>').attr('src', data.slides[j])));
		    }

		    if (this.options.isVideoBakcground) {
		        packet.append(
                    $('<span>').addClass('sponsor imageContainer').append(
                        $($('<video>').append(
                            $('<source>').attr("src", "assets/images/ads/gmc.mp4")
                                        .attr("type", "video/mp4")
                                ).append(
                            $('<source>').attr("src", "assets/images/ads/gmc.ogg")
                                        .attr("src", "video/ogg")
                                )
                            )
                        )
		        );
		    } else {
		        packet.append(
                    $('<span>').addClass('sponsor imageContainer').append(
		                    $($('<img>').attr('src', data.sponsor))
                    )
                );
		    }

		    this.element.find('#imagePackets').append(packet);
		    this._setupPacket(packet);
		},
		
		_start: function () {
			this.element.css({
				'-webkit-animation-duration': (this.options.animationDuration + 'ms'),
	  			'animation-duration': (this.options.animationDuration + 'ms')
			});

			this.currentPacket = this.element.find('.packet').first();
			this.currentPacket.addClass('opaque');
			
			this.slideIndex = 0;
/* 			this.intervalID = setInterval($.proxy(this._cycle, this), this.options.interval);   */
            //Above seems correct to me
            //commenting out below, re-doing interval
			//this._cycle();
			//console.log("Starting cycle interval");
 			this.intervalID = setInterval($.proxy(this._cycle, this), this.options.interval);
		},	
									
		_cycle: function() {		
            /*
            //no longer clearing timeout
			clearTimeout(this.intervalID);
			console.log("clearTimeout " + this.intervalID);
            */
		    //console.log("Cycle");
			var currentSlide = $(this.currentPacket.children('.imageContainer').get(this.slideIndex));
			var previousSlide = $(this.currentPacket.children('.imageContainer').get(this.slideIndex - 1));
			
			// console.log(this.slideIndex);			 
			if (this.slideIndex <= 9) { // Normal slide							
			    if (this.slideIndex === 0) { // Request new packets when displaying first slide.
			        this._setPacketStatus(this.currentPacket.attr("data-packetid"), $.proxy(function () {
                        //managing requestRefresh at the root level
			            //this._requestRefresh();
			            currentSlide.addClass('fadeIn');
                        /*
                        //no longer manually cycling
			            this.intervalID = setInterval($.proxy(this._cycle, this), this.options.interval);
                        console.log("setInterval proxy cycle" + this.intervalID);
                        */
			        }, this)); 					 					
				} else {				
 					this.currentPacket.find('.grid').addClass('opaque');
										
					var gridCell = $(this.currentPacket.find('.grid .imageContainer').get(this.slideIndex - 1));					
/* 					var animationEndEvents = 'animationend webkitAnimationEnd oanimationend msAnimationEnd'; */
																				
					previousSlide &&
						previousSlide
							 // First element pops into place instead of animating smoothly unless I do this first
							 // (Could also use this.slideAnchorPoint(previousSlide) HERE to have the slide move linearly)								
							.transition({
								transformOrigin: "50% 50%"									
							})
							.transition({
								scale: .333333333,		
								transformOrigin: this.slideAnchorPoint(previousSlide)								
							}, 
							this.options.transitionDuration, //800, //
							'ease-in-out',
							$.proxy(function() {
									previousSlide.removeClass('fadeIn').addClass('fadeOut');
									gridCell.addClass('opaque');
									setTimeout($.proxy(function() {
										currentSlide.addClass('fadeIn');
                                        /*
                                        //No longer manually cycling
										this.intervalID = setInterval($.proxy(this._cycle, this), this.options.interval);
                                        console.log("setInterval proxy cycle " + this.intervalID);
                                        */
									}, this), this.options.gridShowDuration)
                                    //console.log("setTimeout slide " + this.intervalID);
							}, this));
				}			 		
			} else if (this.slideIndex === 10) {        			
				this._removeGrid();
                /*
                //No longer manually cycling
				this.intervalID = setInterval($.proxy(this._cycle, this), this.options.interval);
                console.log("setInterval proxy cycle" + this.intervalID);
                */
			} else {		
				this.currentPacket.addClass('stale');		
 				this._switchPacket();				
			}

			this.slideIndex++;
		},

		_setPacketStatus: function(packetId, callback)
		{
		    var model = {
		        PacketId: packetId,
                Status: "viewed"
		    };
		    var promise = $.ajax({
		        url: "API/Photo/Packet/Status",
		        data: model,
		        type: "POST",
		        context: this,
		        dataType: 'json'
		    });

		    promise.done($.proxy(function (json) {
		        var success = json.success;
		        if (success) {
		            var packet = $("div[data-packetid=" + packetId + "]");
		            packet.attr("data-status", "viewed");		            
		        }

		        callback();
		    }, this));

		    promise.error($.proxy(function (json) {
		        callback();
		    }, this));
		},
		
		_flipZ: function (elements) {
			elements.each(function(index) {
				$(this).css({'z-index': $(this).siblings().length - index});
			});
		},
		
		_sizeFrame: function(e) {
			var frame = $('#slideshow').find('.frame'); 
			var height = frame.css('height');			
		    frame.css('width', frame.css('height'));
			
			$('<h4>').css('left', (frame.css('height') - 50) + "px");
		},
						
		_removeGrid: function () {			
			var grid = this.currentPacket.find('.grid');   		  			
			this._transitionGrid(grid, 'fadeOut', 'fadeIn');
		},
		
		_transitionGrid: function(grid, newAnimation, oldAnimation) {
			var iterations = 5; // (3 + 3) - 1;
			var gridStagger = this.options.gridStagger;
			
			grid.find('span').each(function(index) {  
  		  		// Apply diagonally from top-left to bottom-right	
  		  		var column = Math.floor(index / 3);
  		  		var row = (index % 3);	  	
				$(this).delay((column + row) * gridStagger).queue(function(next) {
    				$(this).addClass(newAnimation).removeClass(oldAnimation);
    				next();
				});
  		  	});
		},

		_switchPacket: function () {
			this._trigger('willSwitchPacket');
            /*
            //no longer clearing timeout
			clearTimeout(this.intervalID);
			console.log("clearTimeout " + this.intervalID);
            */
						
			//if (this.element.find('.packet').length <= 1) {
			//	// This does cause an abrupt transition when there is only one packet,
			//	// but it's a LOT simpler and safer than adding a bunch of handling.
			//	this._finalizePacketTransition();
			//} else {
				var newPackets = this.element.find('.packet:not(.stale)');
				var nextPacket = 
					newPackets.length > 0 ? 			 		// Do we have packets that aren't stale (haven't been shown)?
					newPackets.first() : 						// Show the first one of those then.
					(this.currentPacket.next().length > 0 ?		// No? Well, is there an old packet after this one?
						 this.currentPacket.next() :			// Guess we'll settle for that.
						 this.element.find('.packet').first())	// Gah! Start over. We'll jump to a fresh one when we get it.
									
				this.currentPacket
					.removeClass('fadeIn opaque')
					.addClass('lightSpeedOut stale');
	 				
	 			this.currentPacket = nextPacket;
	 			// Clean up the other packets after animating this one in.
	 			this.currentPacket
	 				.one('animationend webkitAnimationEnd oanimationend msAnimationEnd', 
	 					 $.proxy(this._finalizePacketTransition, this));
	 					 
	 			// There are reports about 'animationend' callbacks being unreliable. This is a fallback.
		    /*  	 			this.packetTimeoutID = setTimeout($.proxy(this._finalizePacketTransition, this), this.options.animationDuration + 1);  */

	 			this.currentPacket.addClass('fadeIn');	
			//}			
		},
		
		_finalizePacketTransition: function(e) {
			this._trigger('didSwitchPacket');
			this.currentPacket.off();			
/* 			clearTimeout(this.packetTimeoutID); */
				
			this.element.find('.packet.stale > .imageContainer')
				.each(function() {
					var defaultStyles = {
    					'background-image': $(this).css('background-image'),
    					'z-index': $(this).css('z-index')
  					};

					$(this).removeAttr('style').css(defaultStyles);
				});
				
			this.element
				.find('.packet, .packet *')
				.removeClass('lightSpeedOut fadeOut opaque');

			this.slideIndex = 0;	
            /*
            //No longer manually cycling
			this.intervalID = setInterval($.proxy(this._cycle, this), this.options.interval);
            console.log("setInterval proxy cycle" + this.intervalID);
            */
  			this._cycle();  
    	},
    	  	
    	_requestRefresh: function () {
    		// Only make request when we are running out of unseen packets
    		var freshPackets = this.element.find('.packet:not(.stale)');
    		
    		if (freshPackets.length < 2) {
    		    var promise = $.ajax({
    		        url: "API/Photo/Packet?id=" + this.options.wallID,
    		        context: this,
    		        dataType: 'json'
    		    });

    		    promise.done($.proxy(function (json) {
    		        var packet;

    		        if (json.packets) {
                        // no new packets
    		            if (json.packets.length == 0) {
                            // no existing packets
    		                if (freshPackets.length == 0) {
    		                    // remove stale flag from existing packets
    		                    if (this.options.hasStarted) {
    		                        this._recycle();
    		                    } else {
                                    /*
                                    //Commented out calling in create
    		                        setInterval($.proxy(this._requestRefresh, this), 5000);
                                    console.log("setInterval proxy _requestRefresh " + this.intervalID);
                                    */
    		                    }

    		                    //var stale = this.element.find('.stale');
    		                    //stale.removeClass('stale');
    		                    //stale.addClass('recycled');
    		                    //this._flipZ(stale);    		                    
    		                }    		                
    		            } else {    		                		                
                            //console.log("FOUND PACKETS");
    		                var recycled = this.element.find('.recycled');
    		                recycled.addClass('stale');
    		                recycled.removeClass('recycled');

    		                for (var i in json.packets) {
    		                    var data = eval(json.packets[i].Data)[0];
    		                    var packetId = json.packets[i].PacketId;
    		                    var status = json.packets[i].Status;

    		                    this.options.packets.push({ Data: data, PacketId: packetId, Status: status });

    		                    this._buildPacket(data, packetId, status);
    		                }
    		                this._flipZ(this.element.find('.packet'));
    		                this._cleanup();

    		                if (this.options.hasStarted == false) {
    		                    this.options.hasStarted = true;

    		                    this._start();
    		                }
    		            }    		            
    		        } else {
    		            // no new packets
    		            
                             
    		            //var stale = this.element.find('.stale');
    		            //stale.removeClass("stale");
    		            //this._flipZ(stale);
    		        }
                    //------------------
                    //once we get packets, start the cycle intervals
                    if (this.options.packets.length > 0 && !this.options.hasStarted ) {
                        this.options.hasStarted = true;
                        //console.log("About to start");
                        this._start();
                    }
                    //------------------
    		    }, this));
    		}
    	},

    	_recycle: function() {
    	    var stale = this.element.find('.stale');
    	    stale.removeClass('stale');
    	    stale.addClass('recycled');
    	    this._flipZ(stale);
    	},
    	
    	// Remove stale packets
    	_cleanup: function () {
    		var stalePackets = this.element.find('.packet.stale').not(this.currentPacket);
    		stalePackets.slice(this.options.maxStalePackets, stalePackets.length).remove();    		
    	}
	});
	
	$.fn.extend({
		// Calling chunkedWrap(1, …) is equivalent to using jQuery's .wrap() (but slower).
		// chunkedWrap(3, …) on set [1, 2, 3, … 8] would give you three wrapped sets:
		// [1, 2, 3,], [4, 5, 6], [7, 8]  	  
		chunkedWrap: function(chunkSize, html) {
			if ($.isFunction(html)) {
				return this.each(function(i) {
					$(this).chunkedWrap( html.call(this, i) );
				});
			}					
			
			var chunks = [],
			i = 0;

			while (i < this.length) {
				chunks.push(this.slice(i, i += chunkSize)); 
			}
		
			// Return the entire node (including wrapper) in order to maintain chainability.			 
			return chunks.map(function(chunk) {
				return chunk.wrapAll(html).parent(); // Include the ancestor element we just added
			}).reduce(function(previousValue, currentValue) {
				return currentValue.add(previousValue);
			});	
		}
	});	
})( jQuery, window, document );